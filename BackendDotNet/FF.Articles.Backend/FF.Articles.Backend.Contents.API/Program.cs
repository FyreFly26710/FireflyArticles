using FF.Articles.Backend.Common.Extensions;
using FF.Articles.Backend.Common.Middlewares;
using FF.Articles.Backend.Contents.API;
using FF.Articles.Backend.Contents.API.MqConsumers;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.AddServiceDefaults();
if (builder.Environment.IsDevelopment())
{
    // Only run migrations in development
    builder.Services.AddMigration<ContentsDbContext, ContentsDbSeed>();
}

var connectionString = configuration.GetConnectionString("contentdb") ?? throw new Exception("contentdb connection string not found");
builder.AddNpgsql<ContentsDbContext>(connectionString);

builder.AddRedisClient("redis");

builder.Services.AddContentsServices();

builder.AddRabbitMq();
builder.Services.AddHostedService<UpdateArticleConsumer>();
builder.Services.AddHostedService<AddArticleConsumer>();

// Add FluentValidation
builder.Services.AddFluentValidation();

builder.Services.AddControllers();

builder.AddBasicApi();

//builder.Services.AddElasticsearch(configuration);


var app = builder.Build();

// var scope = app.Services.CreateScope();
// var esService = scope.ServiceProvider.GetRequiredService<IElasticSearchArticleService>();
// var esSyncService = scope.ServiceProvider.GetRequiredService<ElasticsearchSyncService>();
// if (app.Environment.IsDevelopment())
// {
//     var articleIndexCount = await esService.GetArticleIndexCount();
//     if (articleIndexCount == 0)
//     {
//         await esService.ReCreateIndexAsync();
//         await esSyncService.IndexAllExistingArticles();
//     }
// }

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseBasicSwagger();

app.UseHttpsRedirection();

app.AddCookieAuthMiddleware();

app.MapControllers();



app.Run();
