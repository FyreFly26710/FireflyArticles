using FF.Articles.Backend.AI;
using FF.Articles.Backend.AI.API.Infrastructure;
using FF.Articles.Backend.AI.API.Interfaces.Repositories;
using FF.Articles.Backend.AI.API.Interfaces.Services;
using FF.Articles.Backend.AI.API.Interfaces.Services.RemoteServices;
using FF.Articles.Backend.AI.API.Repositories;
using FF.Articles.Backend.AI.API.Services;
using FF.Articles.Backend.AI.API.Services.RemoteServices;
using FF.Articles.Backend.AI.API.UnitOfWork;
using FF.Articles.Backend.Common.Middlewares;
using FF.Articles.Backend.ServiceDefaults;
using Microsoft.EntityFrameworkCore;
using FF.Articles.Backend.Common.Extensions;
using FF.AI.Common;
using FF.Articles.Backend.Common.Constants;
using FF.Articles.Backend.AI.API.Services.Consumers;
using FF.Articles.Backend.RabbitMQ;


var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddDbContext<AIDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("aidb");

    options.UseNpgsql(connectionString)
           .LogTo(Console.WriteLine, LogLevel.Information)
           .EnableSensitiveDataLogging();
});

builder.AddRedisClient("redis");


builder.AddServiceDefaults();
builder.Services.AddHttpClient<IContentsApiRemoteService, ContentsApiRemoteService>();
builder.Services.AddMigration<AIDbContext>();

builder.Services.AddScoped<IAIDbContextUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IChatRoundRepository, ChatRoundRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<ISystemMessageRepository, SystemMessageRepository>();

// Services
builder.Services.AddAI(configuration);
builder.Services.AddScoped<IArticleGenerationService, ArticleGenerationService>();
builder.Services.AddScoped<IChatRoundService, ChatRoundService>();
builder.Services.AddScoped<ISessionService, SessionService>();

builder.AddRabbitMq();
builder.Services.AddHostedService<GenerateArticleConsumer>();


// Stores

builder.Services.AddControllers();

builder.AddBasicApi();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseBasicSwagger();

app.UseHttpsRedirection();

app.AddCookieAuthMiddleware();

app.MapControllers();

app.Run();
