using FF.Articles.Backend.AI.API.MessageConsumers;
using FF.Articles.Backend.Common.Extensions;
using FF.Articles.Backend.Common.Middlewares;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var connectionString = configuration.GetConnectionString("aidb") ?? throw new Exception("aidb connection string not found");
builder.AddNpgsql<AIDbContext>(connectionString);
builder.AddRedisClient("redis");


builder.AddServiceDefaults();
builder.Services.AddHttpClient<IContentsApiRemoteService, ContentsApiRemoteService>();
builder.Services.AddMigration<AIDbContext>();

builder.Services.AddScoped<IAIDbContextUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IChatRoundRepository, ChatRoundRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<ISystemMessageRepository, SystemMessageRepository>();

// Services
builder.Services.AddAI();
builder.Services.AddScoped<IArticleGenerationService, ArticleGenerationService>();
builder.Services.AddScoped<IChatRoundService, ChatRoundService>();
builder.Services.AddScoped<ISessionService, SessionService>();

builder.AddRabbitMq();
builder.Services.AddHostedService<GenerateArticleConsumer>();
builder.Services.AddHostedService<GenerateTopicArticleConsumer>();
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
