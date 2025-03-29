var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis")
    .WithLifetime(ContainerLifetime.Persistent);

var postgres = builder.AddPostgres("postgres")
    .WithImage("ankane/pgvector")
    .WithImageTag("latest")
    .WithLifetime(ContainerLifetime.Persistent); ;

var identityDb = postgres.AddDatabase("identitydb");
var contentDb = postgres.AddDatabase("contentdb");

var launchProfileName = "http";

// Services
builder.AddProject<Projects.FF_Articles_Backend_Identity_API>("identity-api", launchProfileName)
    .WithReference(identityDb)
    .WithReference(redis);

builder.AddProject<Projects.FF_Articles_Backend_Contents_API>("contents-api", launchProfileName)
    .WithReference(contentDb)
    .WithReference(redis);

builder.AddProject<Projects.FF_Articles_Backend_AI_API>("ai-api", launchProfileName);

builder.Build().Run();
