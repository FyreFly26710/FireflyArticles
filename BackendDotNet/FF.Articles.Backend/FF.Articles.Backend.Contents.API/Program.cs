using FF.Articles.Backend.Contents.API;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.ServiceDefaults;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.AddServiceDefaults(configuration);

builder.Services.AddDbContext<ContentsDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    .LogTo(Console.WriteLine, LogLevel.Information)
    .EnableSensitiveDataLogging();
});
var redis = configuration["Redis:ConnectionString"];
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(redis));


builder.Services.AddServices();

builder.Services.AddControllers();


builder.AddCustomApiVersioning();
builder.AddApiVersioningSwagger();

var app = builder.Build();

app.UseCustomSwagger();

app.UseHttpsRedirection();

app.AddCookieAuthMiddleware();

app.MapControllers();

app.Run();
