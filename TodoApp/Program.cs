using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System;
using TodoApp.Repo;
using UserManagement.DependencyExtensions;

var builder = WebApplication.CreateBuilder(args);
#region Dependency Injection for entity framework core implementation (Infrastructure)
// Register DbContext
builder.Services.AddDbContext<EFDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnectionString")));

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var options = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379");
    options.AbortOnConnectFail = false; // keep retrying
    return ConnectionMultiplexer.Connect(options);
});

builder.Services.AddServices();
// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));
#endregion
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
