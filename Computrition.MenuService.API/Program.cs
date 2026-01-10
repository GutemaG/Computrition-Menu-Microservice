using System.Data;
using Computrition.MenuService.API.Data;
using Computrition.MenuService.API.Repositories;
using Computrition.MenuService.API.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Missing connection string 'ConnectionStrings:DefaultConnection' in appsettings.");
}

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(connectionString));
// Register Dapper Connection
builder.Services.AddScoped<IDbConnection>(sp =>
    new SqliteConnection(connectionString));


// Register layers
builder.Services.AddScoped<IMenuRepository, MenuRepository>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IPatientService, PatientService>();

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


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
