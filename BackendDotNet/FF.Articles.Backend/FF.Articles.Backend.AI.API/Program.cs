using FF.Articles.Backend.AI;
using FF.Articles.Backend.AI.API.Interfaces;
using FF.Articles.Backend.AI.API.Interfaces.Services;
using FF.Articles.Backend.AI.API.Interfaces.Services.RemoteServices;
using FF.Articles.Backend.AI.API.Services;
using FF.Articles.Backend.AI.API.Services.RemoteServices;
using FF.Articles.Backend.AI.API.Services.Stores;
using FF.Articles.Backend.Common.Middlewares;
using FF.Articles.Backend.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.AddServiceDefaults(configuration);

// Services
builder.Services.AddDeepSeek(configuration);
builder.Services.AddScoped<IArticleGenerationService, ArticleGenerationService>();
builder.Services.AddScoped<IContentsApiRemoteService, ContentsApiRemoteService>();
builder.Services.AddScoped<IChatService, ChatService>();

// Stores
builder.Services.AddSingleton<UserArticleStateStore>();
builder.Services.AddSingleton<UserChatStateStore>();

builder.Services.AddControllers();

builder.AddBasicApi();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseBasicSwagger();

app.UseHttpsRedirection();

app.AddCookieAuthMiddleware();

app.MapControllers();

app.Run();
