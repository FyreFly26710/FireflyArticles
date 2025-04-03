using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FF.Articles.Backend.ServiceDefaults;
public static class AuthenticationExtensions
{
    public static WebApplicationBuilder AddCookieAuth(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "AuthCookie";
                //options.ExpireTimeSpan = TimeSpan.FromDays(7); // optional
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.None;
            });
        builder.Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".firefly-articles", "data-protection")))
            .SetApplicationName("SharedAuth");
        return builder;
    }

    public static WebApplicationBuilder AddCustomCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowedOrigins",
                builder =>
                {
                    builder.AllowAnyHeader()
                           .AllowAnyMethod()
                           .WithOrigins(new Domain().URL,
                                "http://localhost:3000",
                                "http://localhost:21000",
                                "http://localhost:22000",
                                "http://localhost:23000",
                                "http://localhost:24000")
                           .AllowCredentials();
                });
        });
        return builder;
    }
    public static void AddCookieAuthMiddleware(this WebApplication app)
    {
        app.UseCors("AllowedOrigins");
        app.UseAuthentication();
        app.UseAuthorization();

    }

    
}

public class Domain
{
    public virtual string URL { get; set; } = "";
}
