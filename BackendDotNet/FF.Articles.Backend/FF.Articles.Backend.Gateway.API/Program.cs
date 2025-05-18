using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Forwarder;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

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

builder.WebHost.ConfigureKestrel(options => { options.Limits.MaxRequestBodySize = 104857600; }); // 100MB

var app = builder.Build();

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
        HttpRequest = config.Timeout.HasValue
            ? new ForwarderRequestConfig { ActivityTimeout = config.Timeout.Value }
            : null
    }).ToList();

    return clusterConfigs;
}

class ClusterConf
{
    public string ClusterId { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public TimeSpan? Timeout { get; set; }
}
