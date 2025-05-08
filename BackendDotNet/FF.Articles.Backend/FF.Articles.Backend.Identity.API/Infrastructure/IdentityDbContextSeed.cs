using FF.Articles.Backend.Common.Constants;
using FF.Articles.Backend.Identity.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using FF.Articles.Backend.Common.BackgoundJobs;
using FF.Articles.Backend.Common.ApiDtos;

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
                var defaultUser = AdminUsers.SYSTEM_ADMIN_FIREFLY;

                await context.Users.AddAsync(CreateUser(defaultUser, "0732d07230a94c9318ecdfc223dfb310"));//12345678
                await context.Users.AddAsync(CreateUser(AdminUsers.SYSTEM_ADMIN_DEEPSEEK, "11111111"));
                await context.Users.AddAsync(CreateUser(AdminUsers.SYSTEM_ADMIN_GEMINI, "11111111"));
                await context.SaveChangesAsync();
            }
        }
        private User CreateUser(UserApiDto user, string password)
        {
            return new User
            {
                Id = user.UserId,
                UserName = user.UserName,
                UserAccount = user.UserAccount,
                UserRole = user.UserRole,
                UserAvatar = user.UserAvatar,
                UserProfile = user.UserProfile,
                UserEmail = user.UserEmail,
                UserPassword = password,
            };
        }
    }
}
