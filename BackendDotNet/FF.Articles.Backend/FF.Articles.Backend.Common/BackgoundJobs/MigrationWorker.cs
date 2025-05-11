using Microsoft.Extensions.Hosting;
using Npgsql;

namespace FF.Articles.Backend.Common.BackgoundJobs;

public interface IDbSeeder<in TContext> where TContext : DbContext
{
    Task SeedAsync(IServiceProvider serviceProvider);
}

public static class MigrationWorker
{
    public class MigrationHostedService<TContext>(IServiceProvider serviceProvider) : BackgroundService where TContext : DbContext
    {
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return serviceProvider.MigrateDbContextAsync<TContext>();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }

    private static async Task MigrateDbContextAsync<TContext>(this IServiceProvider services) where TContext : DbContext
    {
        using var scope = services.CreateScope();
        var scopeServices = scope.ServiceProvider;
        var logger = scopeServices.GetRequiredService<ILogger<TContext>>();
        var context = scopeServices.GetService<TContext>()!;
        var seeder = scopeServices.GetService<IDbSeeder<TContext>>();

        try
        {
            logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);

            var connectionString = context.Database.GetConnectionString();
            var databaseName = context.Database.GetDbConnection().Database;
            var masterConnectionString = new NpgsqlConnectionStringBuilder(connectionString) { Database = "postgres" }.ToString();

            // Wait for postgres and create database if needed
            var retryCount = 0;
            var maxRetries = 10;
            var delay = TimeSpan.FromSeconds(4);
            var isInit = true;

            while (retryCount < maxRetries)
            {
                try
                {
                    using var masterConnection = new NpgsqlConnection(masterConnectionString);
                    await masterConnection.OpenAsync();

                    // Try to create database (will fail silently if it exists)
                    var createCommand = masterConnection.CreateCommand();
                    createCommand.CommandText = $"CREATE DATABASE \"{databaseName}\"";
                    try
                    {
                        await createCommand.ExecuteNonQueryAsync();
                        logger.LogInformation("Created database {DatabaseName}", databaseName);
                    }
                    catch (PostgresException ex) when (ex.SqlState == "42P04")
                    {
                        isInit = false;
                        logger.LogInformation("Database {DatabaseName} already exists", databaseName);
                    }

                    // Verify we can connect to the target database
                    if (await context.Database.CanConnectAsync())
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount == maxRetries)
                    {
                        logger.LogError(ex, "Failed to setup database after all retry attempts");
                        throw;
                    }
                    logger.LogWarning(ex, $"Database setup attempt {retryCount} failed");
                    await Task.Delay(delay);
                }
            }

            // Apply migrations
            await context.Database.MigrateAsync();

            // Seed if first init
            if (seeder != null && isInit)
            {
                await seeder.SeedAsync(services);
            }
            logger.LogInformation("Database migration and seeding completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(TContext).Name);
            throw;
        }
    }
}