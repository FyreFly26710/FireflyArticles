using FF.Articles.Backend.Common.Middlewares;
using FF.Articles.Backend.Contents.API;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.ServiceDefaults;
using Microsoft.EntityFrameworkCore;
using FF.Articles.Backend.Common.Extensions;
using FF.Articles.Backend.Contents.API.MqConsumers;
using FF.Articles.Backend.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.AddServiceDefaults();

builder.Services.AddMigration<ContentsDbContext, ContentsDbSeed>();

var connectionString = configuration.GetConnectionString("contentdb") ?? throw new Exception("contentdb connection string not found");
builder.AddNpgsql<ContentsDbContext>(connectionString);

builder.AddRedisClient("redis");

builder.Services.AddContentsServices();

builder.AddRabbitMq();
builder.Services.AddHostedService<UpdateArticleConsumer>();
builder.Services.AddHostedService<AddArticleConsumer>();

builder.Services.AddControllers();

builder.AddCustomApiVersioning();


var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseCustomSwagger();

app.UseHttpsRedirection();

app.AddCookieAuthMiddleware();

app.MapControllers();

app.Run();
