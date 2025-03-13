using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.RemoteServices;
using FF.Articles.Backend.Contents.API.RemoteServices.Interfaces;
using FF.Articles.Backend.Contents.API.Repositories;
using FF.Articles.Backend.Contents.API.Repositories.Interfaces;
using FF.Articles.Backend.Contents.API.Services;
using FF.Articles.Backend.Contents.API.Services.Interfaces;
using FF.Articles.Backend.ServiceDefaults;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(builder.Configuration);
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


builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IArticleTagService, ArticleTagService>();
builder.Services.AddScoped<ITopicService, TopicService>();

builder.Services.AddScoped<IIdentityRemoteService, IdentityRemoteService>();

builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<ITopicRepository, TopicRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IArticleTagRepository, ArticleTagRepository>();


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
