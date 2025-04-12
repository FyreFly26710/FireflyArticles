using FF.Articles.Backend.Common.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FF.Articles.Backend.ServiceDefaults;
public static class ProgramExtensions
{
    public static WebApplicationBuilder AddServiceDefaults(this WebApplicationBuilder builder)
    {
        builder.AddLogger();
        builder.AddCustomCors();

        // Configure cookie authentication
        builder.AddCookieAuth();

        // Register TokenService
        builder.Services.AddSingleton<ITokenService, TokenService>();

        // Add JWT authentication
        builder.AddApiAuthentication();

        // Configure the default authentication scheme to try JWT first, then cookie
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = "MultiScheme";
            options.DefaultChallengeScheme = "MultiScheme";
        })
        .AddPolicyScheme("MultiScheme", "Cookie or JWT", options =>
        {
            options.ForwardDefaultSelector = context =>
            {
                // Check if the request has Authorization header with Bearer token
                if (context.Request.Headers.ContainsKey("Authorization") &&
                    context.Request.Headers["Authorization"].ToString().StartsWith("Bearer "))
                {
                    return JwtBearerDefaults.AuthenticationScheme;
                }

                // Otherwise use cookie authentication
                return CookieAuthenticationDefaults.AuthenticationScheme;
            };
        });

        builder.Services.AddHttpClient();

        return builder;
    }

    public static WebApplicationBuilder AddLogger(this WebApplicationBuilder builder)
    {
        builder.Logging.AddConsole();
        return builder;
    }
    //public static WebApplicationBuilder AddEFSqlServer<TContext>(this WebApplicationBuilder builder, string connectionString) 
    //    where TContext:DbContext
    //{
    //    builder.Services.AddDbContext<TContext>(options =>
    //    {
    //        options.UseSqlServer(connectionString)
    //        .LogTo(Console.WriteLine, LogLevel.Information)
    //        .EnableSensitiveDataLogging();
    //    });
    //    return builder;
    //}
}
