using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                options.Cookie.SecurePolicy = CookieSecurePolicy.None; // only for localhost
                options.Cookie.SameSite = SameSiteMode.Lax; // Allows cross-API requests on localhost
                //options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Ensure this is set for production (HTTPS)
                //options.Cookie.SameSite = SameSiteMode.Lax; // Secure cookie behavior
            });
        builder.Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(@"D:\202502"))
            .SetApplicationName("SharedAuth");
        return builder;
    }

    public static WebApplicationBuilder AddCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowedOrigins",
                builder =>
                {
                    builder.AllowAnyHeader()
                           .AllowAnyMethod()
                           //.AllowAnyOrigin()
                           .WithOrigins("http://localhost:22000", "http://localhost:23000")
                           .AllowCredentials();
                });
        });
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowedBackendUrls",
                builder =>
                {
                    builder.AllowAnyHeader()
                           .AllowAnyMethod()
                           //.AllowAnyOrigin()
                           .WithOrigins("http://localhost:22000", "http://localhost:23000")
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

