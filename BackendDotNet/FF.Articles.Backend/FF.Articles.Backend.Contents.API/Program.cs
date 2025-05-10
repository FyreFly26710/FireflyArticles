using FF.Articles.Backend.Common.Middlewares;
using FF.Articles.Backend.Contents.API;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.ServiceDefaults;
using Microsoft.EntityFrameworkCore;
using FF.Articles.Backend.Common.Extensions;
using FF.Articles.Backend.Contents.API.MqConsumers;
using FF.Articles.Backend.RabbitMQ;
using FF.Articles.Backend.Contents.API.ElasticSearch;

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

builder.Services.AddElasticsearch(configuration);


var app = builder.Build();

var scope = app.Services.CreateScope();
var esService = scope.ServiceProvider.GetRequiredService<IElasticSearchArticleService>();
var esSyncService = scope.ServiceProvider.GetRequiredService<ElasticsearchSyncService>();
if (app.Environment.IsDevelopment())
{
    var articleIndexCount = await esService.GetArticleIndexCount();
    if (articleIndexCount == 0)
    {
        await esService.ReCreateIndexAsync();
        await esSyncService.IndexAllExistingArticles();
    }
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseCustomSwagger();

app.UseHttpsRedirection();

app.AddCookieAuthMiddleware();

app.MapControllers();



app.Run();
