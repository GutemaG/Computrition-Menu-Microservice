using System.Data;
using Computrition.MenuService.API.Data;
using Computrition.MenuService.API.MultiTenancy;
using Computrition.MenuService.API.Repositories;
using Computrition.MenuService.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
       // Header-based "apiKey" security scheme (shows an Authorize button in Swagger UI)
    c.AddSecurityDefinition("Tenant", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Name = TenantHeaderMiddleware.HeaderName, // "X-Tenant-Id"
        Description = "Tenant selector header. Example values: hospital-a, hospital-b"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Tenant"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite("Data Source=menu.db"));
// Register Dapper Connection
builder.Services.AddScoped<IDbConnection>(sp =>
    new Microsoft.Data.Sqlite.SqliteConnection("Data Source=menu.db"));


// Register layers
builder.Services.AddScoped<IMenuRepository, MenuRepository>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IPatientService, PatientService>();

builder.Services.AddScoped<IHospitalRepository, HospitalRepository>();
builder.Services.AddScoped<IHospitalService, HospitalService>();

// MultiTenancy Dependency
builder.Services.AddScoped<ITenantContext, TenantContext>();
builder.Services.AddScoped<TenantHeaderMiddleware>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Automatically create the database and apply seed data on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    // This creates the .db file if it doesn't exist
    context.Database.EnsureCreated(); 
}

// add multi tenancy middleware
app.UseMiddleware<TenantHeaderMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
