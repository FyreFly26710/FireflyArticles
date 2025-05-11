namespace FF.Articles.Backend.Common.Extensions;
public static class ServiceDefaultsExtensions
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

        // Filter out LogicalHandler logs but keep ClientHandler logs
        builder.Logging.AddFilter("System.Net.Http.HttpClient.*.LogicalHandler", LogLevel.Warning);

        return builder;
    }
    public static WebApplicationBuilder AddNpgsql<TContext>(this WebApplicationBuilder builder, string connectionString)
       where TContext : DbContext
    {
        builder.Services.AddDbContext<TContext>(options =>
        {
            options.UseNpgsql(connectionString)
                   .EnableSensitiveDataLogging();
        });
        return builder;
    }
}
