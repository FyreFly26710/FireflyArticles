using FF.Articles.Backend.AI;
using FF.Articles.Backend.AI.API.Infrastructure;
using FF.Articles.Backend.AI.API.Interfaces;
using FF.Articles.Backend.AI.API.Interfaces.Repositories;
using FF.Articles.Backend.AI.API.Interfaces.Services;
using FF.Articles.Backend.AI.API.Interfaces.Services.RemoteServices;
using FF.Articles.Backend.AI.API.Repositories;
using FF.Articles.Backend.AI.API.Services;
using FF.Articles.Backend.AI.API.Services.RemoteServices;
using FF.Articles.Backend.AI.API.Services.Stores;
using FF.Articles.Backend.AI.API.UnitOfWork;
using FF.Articles.Backend.Common.Middlewares;
using FF.Articles.Backend.ServiceDefaults;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddDbContext<AIDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("aidb");

    options.UseNpgsql(connectionString)
           .LogTo(Console.WriteLine, LogLevel.Information)
           .EnableSensitiveDataLogging();
});


builder.AddServiceDefaults();

builder.Services.AddScoped<IAIDbContextUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IChatRoundRepository, ChatRoundRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<ISystemMessageRepository, SystemMessageRepository>();

// Services
builder.Services.AddDeepSeek(configuration);
builder.Services.AddScoped<IArticleGenerationService, ArticleGenerationService>();
builder.Services.AddScoped<IContentsApiRemoteService, ContentsApiRemoteService>();
builder.Services.AddScoped<IChatService, ChatService>();

// Stores
builder.Services.AddSingleton<UserArticleStateStore>();

builder.Services.AddControllers();

builder.AddBasicApi();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseBasicSwagger();

app.UseHttpsRedirection();

app.AddCookieAuthMiddleware();

app.MapControllers();

app.Run();
