using Microsoft.Extensions.Configuration;

namespace FF.Articles.AppHost;
// Add extension method for Elasticsearch configuration
public static class ElasticsearchExtensions
{
    public static IResourceBuilder<ElasticsearchResource> ConfigureElasticsearch(
        this IResourceBuilder<ElasticsearchResource> builder,
        IConfiguration configuration)
    {
        var esConfig = configuration.GetSection("Elasticsearch");

        return builder
            .WithDataVolume()
            .WithLifetime(ContainerLifetime.Persistent)
            .WithHttpEndpoint(port: 9200, targetPort: 9200, name: "es-http")
            // Security settings
            .WithEnvironment("xpack.security.enabled", esConfig["Security:XPackSecurityEnabled"])
            .WithEnvironment("xpack.security.transport.ssl.enabled", esConfig["Security:XPackSecurityTransportSslEnabled"])
            .WithEnvironment("xpack.security.http.ssl.enabled", esConfig["Security:XPackSecurityHttpSslEnabled"])
            .WithEnvironment("discovery.type", "single-node")
            // Memory settings
            .WithEnvironment("ES_JAVA_OPTS", esConfig["Memory:JavaOpts"])
            .WithEnvironment("bootstrap.memory_lock", esConfig["Memory:BootstrapMemoryLock"])
            .WithEnvironment("action.destructive_requires_name", "true")
            // Indices settings
            .WithEnvironment("indices.breaker.total.use_real_memory", esConfig["Indices:BreakerTotalUseRealMemory"])
            .WithEnvironment("indices.breaker.total.limit", esConfig["Indices:BreakerTotalLimit"])
            .WithEnvironment("indices.breaker.fielddata.limit", esConfig["Indices:BreakerFielddataLimit"])
            .WithEnvironment("indices.breaker.request.limit", esConfig["Indices:BreakerRequestLimit"])
            .WithEnvironment("indices.fielddata.cache.size", esConfig["Indices:FielddataCacheSize"])
            .WithEnvironment("indices.queries.cache.size", esConfig["Indices:QueriesCacheSize"])
            .WithEnvironment("search.max_buckets", esConfig["Indices:SearchMaxBuckets"])
            // Thread pool settings
            .WithEnvironment("thread_pool.search.size", esConfig["ThreadPools:SearchSize"])
            .WithEnvironment("thread_pool.write.size", esConfig["ThreadPools:WriteSize"])
            .WithEnvironment("thread_pool.get.size", esConfig["ThreadPools:GetSize"])
            .WithEnvironment("thread_pool.analyze.size", esConfig["ThreadPools:AnalyzeSize"])
            // Script settings
            .WithEnvironment("script.allowed_types", esConfig["Script:AllowedTypes"])
            .WithEnvironment("script.allowed_contexts", esConfig["Script:AllowedContexts"])
            // Cluster settings
            .WithEnvironment("cluster.routing.allocation.disk.threshold_enabled", esConfig["Cluster:RoutingAllocationDiskThresholdEnabled"])
            .WithEnvironment("cluster.routing.allocation.disk.watermark.low", esConfig["Cluster:RoutingAllocationDiskWatermarkLow"])
            .WithEnvironment("cluster.routing.allocation.disk.watermark.high", esConfig["Cluster:RoutingAllocationDiskWatermarkHigh"])
            .WithEnvironment("cluster.routing.allocation.disk.watermark.flood_stage", esConfig["Cluster:RoutingAllocationDiskWatermarkFloodStage"]);
    }
}
