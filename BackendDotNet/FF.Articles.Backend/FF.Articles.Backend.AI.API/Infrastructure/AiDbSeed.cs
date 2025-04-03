using System;
using FF.Articles.Backend.AI.API.Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;

namespace FF.Articles.Backend.AI.API.Infrastructure;

public static class AiDbSeed
{
    public static async Task InitializeDatabase(WebApplicationBuilder builder)
    {
        // Apply pending migrations
        bool? hasPendingMigrations = false;
        using (var scope = builder.Services.BuildServiceProvider().CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AIDbContext>();

            var pendingMigrations = dbContext.Database.GetPendingMigrations();

            hasPendingMigrations = pendingMigrations?.Any();

            dbContext.Database.Migrate();
        }
    }
}