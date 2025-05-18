using Microsoft.AspNetCore.Http.Timeouts;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Forwarder;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

// Log Kestrel settings
Console.WriteLine("=== Kestrel Server Settings ===");
Console.WriteLine($"MaxRequestBodySize: {104857600} bytes (100MB)");
Console.WriteLine($"KeepAliveTimeout: {TimeSpan.FromMinutes(10)}");
Console.WriteLine($"RequestHeadersTimeout: {TimeSpan.FromSeconds(60)}");

builder.Services.AddReverseProxy()
    .LoadFromMemory(GetRoutes(), GetClusters(builder.Configuration))
    .AddTransforms(transforms =>
    {
        transforms.AddRequestTransform(context =>
        {
            context.ProxyRequest.Headers.Add("X-Forwarded-Host", context.HttpContext.Request.Host.Value);
            context.ProxyRequest.Headers.Add("X-Request-Id", context.HttpContext.TraceIdentifier);
            return ValueTask.CompletedTask;
        });
    });

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 104857600; // 100MB
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10);
    options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(60);
});

var app = builder.Build();

// Log routes configuration
Console.WriteLine("\n=== Route Configuration ===");
foreach (var route in GetRoutes())
{
    Console.WriteLine($"Route: {route.RouteId} -> {route.ClusterId} (Path: {route.Match.Path})");
}

// Log timeout values
Console.WriteLine("\n=== Timeout Settings ===");
var clusters = GetClusters(builder.Configuration);
foreach (var cluster in clusters)
{
    Console.WriteLine($"Cluster {cluster.ClusterId}:");
    Console.WriteLine($"  Activity Timeout: {cluster.HttpRequest?.ActivityTimeout}");
}

app.MapReverseProxy();

app.Run();

static IReadOnlyList<RouteConfig> GetRoutes() =>
    [
        new ()
        {
            RouteId = "identity-route",
            ClusterId = "identityCluster",
            Match = new RouteMatch {Path = "/api/identity/{**catch-all}"}
        },
        new ()
        {
            RouteId = "contents-route",
            ClusterId = "contentsCluster",
            Match = new RouteMatch {Path = "/api/contents/{**catch-all}"}
        },
        new ()
        {
            RouteId = "ai-route",
            ClusterId = "aiCluster",
            Match = new RouteMatch {Path = "/api/ai/{**catch-all}"}
        }
    ];

static IReadOnlyList<ClusterConfig> GetClusters(IConfiguration configuration)
{
    var clusters = configuration.GetSection("Clusters").Get<List<ClusterConf>>() ?? new List<ClusterConf>();

    var clusterConfigs = clusters.Select(config => new ClusterConfig
    {
        ClusterId = config.ClusterId,
        Destinations = new Dictionary<string, DestinationConfig>
        {
            { "destination1", new DestinationConfig { Address = config.Address } }
        },
        HttpRequest = new ForwarderRequestConfig { ActivityTimeout = TimeSpan.FromMinutes(10) }
    }).ToList();
    foreach (var cluster in clusterConfigs)
    {
        Console.WriteLine($"Cluster {cluster.ClusterId} timeout: {cluster.HttpRequest?.ActivityTimeout}");
    }
    return clusterConfigs;
}

class ClusterConf
{
    public string ClusterId { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}
