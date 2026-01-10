using System.Data;
using Computrition.MenuService.API.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Computrition.MenuService.Tests.TestInfrastructure;

public sealed class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string ConnectionString = "Data Source=computrition-tests;Mode=Memory;Cache=Shared";
    private readonly SqliteConnection _keepAliveConnection = new(ConnectionString);

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Replace the production DB registrations with an in-memory SQLite database shared across connections.
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
            services.RemoveAll(typeof(AppDbContext));
            services.RemoveAll(typeof(IDbConnection));

            _keepAliveConnection.Open();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite(_keepAliveConnection);
            });

            // Dapper uses its own IDbConnection; use the same shared in-memory DB.
            services.AddScoped<IDbConnection>(_ => new SqliteConnection(ConnectionString));
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _keepAliveConnection.Dispose();
        }

        base.Dispose(disposing);
    }
}
