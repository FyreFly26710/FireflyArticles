
var builder = DistributedApplication.CreateBuilder(args);

//var redis = builder.AddRedis("redis")
//    .WithLifetime(ContainerLifetime.Persistent)
//    .WithEndpoint("tcp", e =>
//    {
//        e.Port = 6380;
//        e.IsProxied = false;
//    })
//    .WithVolume("redis-data", "/data") // Persist Redis data
//    .WithRedisInsight(redisInsight => redisInsight.WithHostPort(8001));

var rabbitMqUsername = builder.AddParameter("rabbitmqUsername", secret: false);
var rabbitMqPassword = builder.AddParameter("rabbitmqPassword", secret: false);
// Default 15672 and 5672, set to 15673 and 5673 to avoid port conflicts
var rabbitMq = builder.AddRabbitMQ("rabbitmq", rabbitMqUsername, rabbitMqPassword)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithManagementPlugin(15673)
    .WithVolume("rabbitmq-data", "/rabbitmqdata")
    .WithEndpoint("tcp", e =>
    {
        e.Port = 5673;
        e.IsProxied = false;
    });

var username = builder.AddParameter("username", secret: false);
var password = builder.AddParameter("password", secret: false);
// Default 5432, set to 5433 to avoid port conflicts
// Todo, data is not yet persisted
var postgres = builder.AddPostgres("postgres", username, password)
    .WithImage("postgres")
    .WithImageTag("15")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpoint("tcp", e =>
    {
        e.Port = 5433;
        e.IsProxied = false;
    })
    .WithEnvironment("POSTGRES_USER", username)
    .WithEnvironment("POSTGRES_PASSWORD", password)
    .WithVolume("postgres-data", "/pgsqldata")
    .WithPgWeb(pgweb => pgweb.WithHostPort(5050));

// Todo: these databases are not created by apphost, need to create them manually
var identityDb = postgres.AddDatabase("identitydb");
var contentDb = postgres.AddDatabase("contentdb");
var aidb = postgres.AddDatabase("aidb");

// var passwordElastic = builder.AddParameter("passwordElastic", secret: false);
// var elasticsearch = builder.AddElasticsearch("elasticsearch", password: passwordElastic)
//     .ConfigureElasticsearch(builder.Configuration);

var launchProfileName = "https";

// Services
builder.AddProject<Projects.FF_Articles_Backend_Identity_API>("identity-api", launchProfileName)
    //.WithReference(redis)
    .WithReference(identityDb);

builder.AddProject<Projects.FF_Articles_Backend_Contents_API>("contents-api", launchProfileName)
    .WithReference(rabbitMq)
    //.WithReference(elasticsearch)
    //.WithReference(redis)
    .WithReference(contentDb);

builder.AddProject<Projects.FF_Articles_Backend_AI_API>("ai-api", launchProfileName)
    .WithReference(rabbitMq)
    //.WithReference(redis)
    .WithReference(aidb);

builder.AddProject<Projects.FF_Articles_Backend_Gateway_API>("gateway-api", launchProfileName);
builder.Build().Run();
