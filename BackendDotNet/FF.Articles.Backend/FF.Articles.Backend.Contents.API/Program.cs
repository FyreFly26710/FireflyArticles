using FF.Articles.Backend.AI;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories;
using FF.Articles.Backend.Contents.API.Interfaces.Services;
using FF.Articles.Backend.Contents.API.Interfaces.Services.RemoteServices;
using FF.Articles.Backend.Contents.API.Repositories;
using FF.Articles.Backend.Contents.API.Services;
using FF.Articles.Backend.Contents.API.Services.AIServices;
using FF.Articles.Backend.Contents.API.Services.RemoteServices;
using FF.Articles.Backend.Contents.API.UnitOfWork;
using FF.Articles.Backend.ServiceDefaults;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.AddServiceDefaults(configuration);
// builder.Services.AddHttpsRedirection(options => {
//     options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
//     options.HttpsPort = 23000; 
// });
builder.Services.AddDbContext<ContentsDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    .LogTo(Console.WriteLine, LogLevel.Information)
    .EnableSensitiveDataLogging();
});

builder.Services.AddDeepSeek(configuration);
builder.Services.AddScoped<ArticleGenerationService>();

builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IArticleTagService, ArticleTagService>();
builder.Services.AddScoped<ITopicService, TopicService>();

builder.Services.AddScoped<IIdentityRemoteService, IdentityRemoteService>();

builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<ITopicRepository, TopicRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IArticleTagRepository, ArticleTagRepository>();

builder.Services.AddScoped<IContentsUnitOfWork, ContentsUnitOfWork>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.CustomOperationIds(apiDesc =>
    {
        if (apiDesc.ActionDescriptor is ControllerActionDescriptor descriptor)
        {
            var resource = descriptor.ControllerName.Replace("Controller", "");
            return $"api{resource}{descriptor.ActionName}";
        }
        return null;
    });
});
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.AddCookieAuthMiddleware();

app.MapControllers();

app.Run();
