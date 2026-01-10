using System.Data;
using System.Net;
using Dapper;

namespace Computrition.MenuService.API.MultiTenancy
{
    public sealed class TenantHeaderMiddleware : IMiddleware
    {
        public const string HeaderName = "X-Tenant-Id";
        private readonly ITenantContext _tenant;
        private readonly IDbConnection _db;
        public TenantHeaderMiddleware(ITenantContext tenant, IDbConnection db)
        {
            _tenant = tenant;
            _db = db;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata.GetMetadata<SkipTenantHeaderAttribute>() is not null)
            {
                await next(context);
                return;
            }
            
            if (!context.Request.Headers.TryGetValue(HeaderName, out var values))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync($"Missing required header: {HeaderName}");
                return;
            }
            var slug = values.FirstOrDefault()?.Trim();
            if (string.IsNullOrWhiteSpace(slug))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync($"Header {HeaderName} cannot be empty.");
                return;
            }
            var hospitalId = await _db.QuerySingleOrDefaultAsync<int?>(
                "Select Id from Hospitals WHERE slug = @Slug", new { Slug = slug }
            );
             if (hospitalId is null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync($"Unknown tenant (hospital slug): {slug}");
                return;
            }
            _tenant.SetTenant(hospitalId.Value, slug);
            await next(context);

        }
    }
}