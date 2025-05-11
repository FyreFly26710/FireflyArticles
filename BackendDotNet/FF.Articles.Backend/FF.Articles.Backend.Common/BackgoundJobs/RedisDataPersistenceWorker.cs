using Microsoft.Extensions.Hosting;

namespace FF.Articles.Backend.Common.BackgoundJobs;

//services.AddHostedService<RedisDataPersistenceWorker<AppDbContext>>();
public class RedisDataPersistenceWorker<TContext> : BackgroundService
    where TContext : DbContext
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConnectionMultiplexer _redis;
    private readonly int _maxRetries = 3;
    private readonly TimeSpan _retryDelay = TimeSpan.FromSeconds(5);

    public RedisDataPersistenceWorker(IServiceScopeFactory scopeFactory, IConnectionMultiplexer redis)
    {
        _scopeFactory = scopeFactory;
        _redis = redis;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessNextChange(stoppingToken);
            }
            catch (Exception ex)
            {
                // Log error and continue
                await Task.Delay(_retryDelay, stoppingToken);
            }
        }
    }

    private async Task ProcessNextChange(CancellationToken stoppingToken)
    {
        var db = _redis.GetDatabase();
        var result = await db.ListLeftPopAsync("DataChangeQueue");

        if (result.IsNullOrEmpty)
        {
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            return;
        }

        for (int attempt = 0; attempt < _maxRetries; attempt++)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<TContext>();

                var change = JsonSerializer.Deserialize<DataChange>(result!);
                if (change != null)
                {
                    await PersistChangeAsync(context, change);
                    await context.SaveChangesAsync(stoppingToken);
                }
                break;
            }
            catch when (attempt < _maxRetries - 1)
            {
                await Task.Delay(_retryDelay, stoppingToken);
            }
        }
    }

    private async Task PersistChangeAsync(TContext context, DataChange change)
    {
        var entityType = Type.GetType(change.FullName);
        if (entityType == null) return;

        var entity = change.PayloadJson != null
            ? JsonSerializer.Deserialize(change.PayloadJson, entityType)
            : null;

        switch (change.ChangeType)
        {
            case ChangeType.Create:
                context.Add(entity!);
                break;
            case ChangeType.Update:
                context.Update(entity!);
                break;
            case ChangeType.Delete:
                var toRemove = await context.FindAsync(entityType, change.Id);
                if (toRemove != null)
                    context.Remove(toRemove);
                break;
        }
    }
}
