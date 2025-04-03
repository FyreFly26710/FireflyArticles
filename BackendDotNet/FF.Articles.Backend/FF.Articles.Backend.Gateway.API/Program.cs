using FF.Articles.Backend.Common.Middlewares;
using Yarp.ReverseProxy.Transforms;

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
            context.ProxyRequest.Headers.Add("X-Forwarded-Host", context.HttpContext.Request.Host.Value);
            context.ProxyRequest.Headers.Add("X-Request-Id", context.HttpContext.TraceIdentifier);
            return ValueTask.CompletedTask;
        });
    });

// Increase Kestrel limits for larger requests/responses
builder.WebHost.ConfigureKestrel(options => { options.Limits.MaxRequestBodySize = 104857600; }); // 100MB

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

//app.UseExceptionHandler(appBuilder =>
//{
//    appBuilder.Run(async context =>
//    {
//        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
//        context.Response.ContentType = "application/json";

//        await context.Response.WriteAsJsonAsync(new
//        {
//            ErrorCode = 50000,
//            RequestId = context.TraceIdentifier
//        });
//    });
//});


app.MapReverseProxy();

app.Run();