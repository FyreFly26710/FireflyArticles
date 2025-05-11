using static FF.Articles.Backend.Common.BackgoundJobs.MigrationWorker;
namespace FF.Articles.Backend.Common.Extensions;

public static class MigrationExtension
{
    public static IServiceCollection AddMigration<TContext, TDbSeeder>(this IServiceCollection services)
        where TContext : DbContext
        where TDbSeeder : class, IDbSeeder<TContext>
    {
        services.AddScoped<IDbSeeder<TContext>, TDbSeeder>();
        services.AddHostedService(sp => new MigrationHostedService<TContext>(sp));
        return services;
    }
    public static IServiceCollection AddMigration<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        services.AddHostedService(sp => new MigrationHostedService<TContext>(sp));
        return services;
    }
}
