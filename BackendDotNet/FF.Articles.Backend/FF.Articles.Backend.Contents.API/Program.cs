using FF.Articles.Backend.Common.Middlewares;
using FF.Articles.Backend.Contents.API;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Infrastructure.Migrations;
using FF.Articles.Backend.ServiceDefaults;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.AddServiceDefaults();

builder.Services.AddDbContext<ContentsDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("contentdb");

    options.UseNpgsql(connectionString)
           .LogTo(Console.WriteLine, LogLevel.Information)
           .EnableSensitiveDataLogging();
});

builder.AddRedisClient("redis");

builder.Services.AddContentsServices();

builder.Services.AddControllers();

builder.AddCustomApiVersioning();

await ContentsDbSeed.InitializeDatabase(builder);

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseCustomSwagger();

app.UseHttpsRedirection();

app.AddCookieAuthMiddleware();

app.MapControllers();

app.Run();
