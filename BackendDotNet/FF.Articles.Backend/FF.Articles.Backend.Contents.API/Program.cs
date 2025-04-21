using FF.Articles.Backend.Common.Middlewares;
using FF.Articles.Backend.Contents.API;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Infrastructure.Migrations;
using FF.Articles.Backend.ServiceDefaults;
using Microsoft.EntityFrameworkCore;
using FF.Articles.Backend.Common.Extensions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.AddServiceDefaults();

builder.Services.AddMigration<ContentsDbContext, ContentsDbSeed>();

builder.Services.AddDbContext<ContentsDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("contentdb");

    options.UseNpgsql(connectionString)
           .LogTo(
               (eventData) => Console.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss.fff} {eventData}"),
               new[] { Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.CommandExecuted },
               LogLevel.Information)
           .EnableSensitiveDataLogging();
});

builder.AddRedisClient("redis");

builder.Services.AddContentsServices();

builder.Services.AddControllers();

builder.AddCustomApiVersioning();


var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseCustomSwagger();

app.UseHttpsRedirection();

app.AddCookieAuthMiddleware();

app.MapControllers();

app.Run();
