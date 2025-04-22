using FF.Articles.Backend.Common.Middlewares;
using Yarp.ReverseProxy.Transforms;
using Microsoft.AspNetCore.Http;

// This is a minimal reverse proxy example using YARP
// It redircts all incoming requests from http://localhost:21000 to the specified backend server configured in the appsettings.json file
// Currently for development purposes, in Prod, use NGINX.

var builder = WebApplication.CreateBuilder(args);

// Add YARP reverse proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(transforms =>
    {
        transforms.AddRequestTransform(context =>
        {
            // Log the transformation for debugging
            Console.WriteLine($"Transforming request: {context.HttpContext.Request.Method} {context.HttpContext.Request.Path}");
            Console.WriteLine($"Destination URL: {context.DestinationPrefix}");

            context.ProxyRequest.Headers.Add("X-Forwarded-Host", context.HttpContext.Request.Host.Value);
            context.ProxyRequest.Headers.Add("X-Request-Id", context.HttpContext.TraceIdentifier);
            return ValueTask.CompletedTask;
        });
    });

// Increase Kestrel limits for larger requests/responses
builder.WebHost.ConfigureKestrel(options => { options.Limits.MaxRequestBodySize = 104857600; }); // 100MB

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

// Add diagnostic middleware
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request received: {context.Request.Method} {context.Request.Path}");
    await next();
    Console.WriteLine($"Response: {context.Response.StatusCode}");
});

app.MapReverseProxy();

// Add a fallback route for debugging
app.Map("/{**catch-all}", async context =>
{
    Console.WriteLine($"Fallback route hit: {context.Request.Path}");
    await context.Response.WriteAsync($"Route not found: {context.Request.Path}. This indicates the reverse proxy didn't handle the request.");
});

app.Run();