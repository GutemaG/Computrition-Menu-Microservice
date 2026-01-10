using System.Data;
using Computrition.MenuService.API.Data;
using Computrition.MenuService.API.Repositories;
using Computrition.MenuService.API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite("Data Source=menu.db"));
// Register Dapper Connection
builder.Services.AddScoped<IDbConnection>(sp =>
    new Microsoft.Data.Sqlite.SqliteConnection("Data Source=menu.db"));


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
