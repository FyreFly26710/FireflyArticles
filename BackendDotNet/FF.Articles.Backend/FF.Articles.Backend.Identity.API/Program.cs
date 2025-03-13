using FF.Articles.Backend.Identity.API.Infrastructure;
using FF.Articles.Backend.Identity.API.MappingProfiles;
using FF.Articles.Backend.Identity.API.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using FF.Articles.Backend.ServiceDefaults;
using Microsoft.AspNetCore.Mvc.Controllers;
using FF.Articles.Backend.Identity.API.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.AddServiceDefaults(builder.Configuration);

builder.Services.AddDbContext<IdentityDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    .LogTo(Console.WriteLine, LogLevel.Information)
    .EnableSensitiveDataLogging();
});
builder.Services.AddHttpClient();

builder.Services.AddAutoMapper(typeof(UserMappingProfile));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOAuthService, OAuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
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
