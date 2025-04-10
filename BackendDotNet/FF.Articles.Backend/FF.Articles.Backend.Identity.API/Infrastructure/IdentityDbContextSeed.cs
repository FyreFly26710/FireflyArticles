using FF.Articles.Backend.Common.Constants;
using FF.Articles.Backend.Identity.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using FF.Articles.Backend.Common.BackgoundJobs;

namespace FF.Articles.Backend.Identity.API.Infrastructure
{
    public class IdentityDbContextSeed : IDbSeeder<IdentityDbContext>
    {
        public async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
            var connection = (NpgsqlConnection)context.Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync();
            }
            // Seed initial data if no users exist
            if (!await context.Users.AnyAsync())
            {
                var defaultUser = new User
                {
                    Id = 1L,
                    UserName = "admin",
                    UserAccount = "firefly",
                    UserRole = UserConstant.ADMIN_ROLE,
                    UserPassword = "0732d07230a94c9318ecdfc223dfb310" // 12345678
                };

                await context.Users.AddAsync(defaultUser);
                await context.SaveChangesAsync();
            }
        }
    }
}
