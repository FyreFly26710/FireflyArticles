var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpoint("tcp", e =>
    {
        e.Port = 6380;
        e.IsProxied = false;
    });

var username = builder.AddParameter("username", secret: false);
var password = builder.AddParameter("password", secret: false);
var postgres = builder.AddPostgres("postgres", username, password)
    .WithImage("postgres")
    .WithImageTag("15")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpoint("tcp", e =>
    {
        e.Port = 5433;
        e.IsProxied = false;
    })
    // .WithEnvironment("POSTGRES_DB", "postgres")
    .WithPgWeb(pgweb =>
    {
        pgweb.WithHostPort(5050);
        pgweb.WithLifetime(ContainerLifetime.Persistent);
    });

var identityDb = postgres.AddDatabase("identitydb");
var contentDb = postgres.AddDatabase("contentdb");
var aidb = postgres.AddDatabase("aidb");

var launchProfileName = "https";

// Services
builder.AddProject<Projects.FF_Articles_Backend_Identity_API>("identity-api", launchProfileName)
    .WithReference(identityDb)
    .WithReference(redis);

builder.AddProject<Projects.FF_Articles_Backend_Contents_API>("contents-api", launchProfileName)
    .WithReference(contentDb)
    .WithReference(redis);

builder.AddProject<Projects.FF_Articles_Backend_AI_API>("ai-api", launchProfileName)
    .WithReference(aidb);

builder.AddProject<Projects.FF_Articles_Backend_Gateway_API>("gateway-api", launchProfileName);
builder.Build().Run();
