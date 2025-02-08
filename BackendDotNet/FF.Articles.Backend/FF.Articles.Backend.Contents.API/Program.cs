using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Services;
using FF.Articles.Backend.Contents.API.Services.Interfaces;
using FF.Articles.Backend.Identity.API.MappingProfiles;
using FF.Articles.Backend.ServiceDefaults;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
//builder.AddEFSqlServer<ArticleDbContext>(builder.Configuration.GetConnectionString("DefaultConnection"));

builder.Services.AddDbContext<ContentsDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    .LogTo(Console.WriteLine, LogLevel.Information)
    .EnableSensitiveDataLogging();
});

builder.Services.AddAutoMapper(typeof(ArticleMappingProfile));

builder.Services.AddScoped<IArticleService, ArticleService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.AddCookieAuthMiddleware();

app.MapControllers();

app.Run();
