using System.Text;
using FF.Articles.Backend.Common.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace FF.Articles.Backend.ServiceDefaults;
public static class AuthenticationExtensions
{
    public static WebApplicationBuilder AddCookieAuth(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication()
        .AddCookie(options =>
        {
            options.Cookie.Name = "AuthCookie";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.None;
            options.Events = new ThrowingCookieAuthenticationEvents();
        });
        builder.Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".firefly-articles", "data-protection")))
            .SetApplicationName("SharedAuth");
        return builder;
    }
    public static WebApplicationBuilder AddApiAuthentication(this WebApplicationBuilder builder)
    {
        // Configure JWT for service-to-service communication
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "your-secret-key-at-least-16-chars-long"));

        builder.Services.AddAuthentication()
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key
                };
                options.Events = new ThrowingJwtBearerEvents();
            });

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
                           .WithOrigins(
                                "http://localhost:3000",
                                // "http://localhost:21000",
                                // "http://localhost:22000",
                                // "http://localhost:23000",
                                // "http://localhost:24000",
                                "http://90.241.0.203:8443"
                                )
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

public class ThrowingCookieAuthenticationEvents : CookieAuthenticationEvents
{
    public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
    {
        // For API requests, throw an exception instead of redirecting
        if (context.Request.Path.StartsWithSegments("/api") ||
            context.Request.Headers["Accept"].Any(h => h.Contains("application/json")))
        {
            throw new ApiException(ErrorCode.NOT_LOGIN_ERROR, "Authentication required");
        }

        return base.RedirectToLogin(context);
    }

    public override Task RedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> context)
    {
        // For API requests, throw an exception instead of redirecting
        if (context.Request.Path.StartsWithSegments("/api") ||
            context.Request.Headers["Accept"].Any(h => h.Contains("application/json")))
        {
            throw new ApiException(ErrorCode.FORBIDDEN_ERROR, "Insufficient permissions");
        }

        return base.RedirectToAccessDenied(context);
    }
}
public class ThrowingJwtBearerEvents : JwtBearerEvents
{
    public override Task Challenge(JwtBearerChallengeContext context)
    {
        // For API requests, throw an exception instead of the default response
        if (context.Request.Path.StartsWithSegments("/api") ||
            context.Request.Headers["Accept"].Any(h => h.Contains("application/json")))
        {
            context.HandleResponse(); // Prevent default challenge response
            throw new ApiException(ErrorCode.NOT_LOGIN_ERROR, "API authentication required");
        }

        return base.Challenge(context);
    }

    public override Task Forbidden(ForbiddenContext context)
    {
        // For API requests, throw an exception instead of the default response
        if (context.Request.Path.StartsWithSegments("/api") ||
            context.Request.Headers["Accept"].Any(h => h.Contains("application/json")))
        {
            throw new ApiException(ErrorCode.FORBIDDEN_ERROR, "API insufficient permissions");
        }

        return base.Forbidden(context);
    }

    public override Task TokenValidated(TokenValidatedContext context)
    {
        // Copy the 'user' claim to make it available in the same way as cookie auth
        var userClaim = context.Principal.FindFirst("user");
        if (userClaim != null)
        {
            // Make sure the claim is added to the identity
            var identity = context.Principal.Identity as ClaimsIdentity;
            identity?.AddClaim(userClaim);
        }
        return Task.CompletedTask;
    }
}