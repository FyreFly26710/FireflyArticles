using FF.Articles.Backend.Common.Constants;
using FF.Articles.Backend.Identity.API.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FF.Articles.Backend.Identity.API.Infrastructure
{
    public static class IdentityDbContextSeed
    {
        public static async Task InitialiseDatabase(WebApplicationBuilder builder)
        {
            bool? hasPendingMigrations = false;
            using (var scope = builder.Services.BuildServiceProvider().CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

                var pendingMigrations = dbContext.Database.GetPendingMigrations();

                hasPendingMigrations = pendingMigrations?.Any();               
                
                dbContext.Database.Migrate();
            }
            if (hasPendingMigrations == true)
            {
                using (var scope = builder.Services.BuildServiceProvider().CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
                    RemoveExisting(dbContext);
                    SeedData(dbContext);
                }
            }
        }
        public static void RemoveExisting(IdentityDbContext context)
        {
            context.Users.RemoveRange(context.Users);
            context.SaveChanges();
        }
        public static void SeedData(IdentityDbContext context)
        {
            if (!context.Users.Any())
            {
                var defaultUser = new User
                {
                    Id = 1L,
                    UserName = "admin",
                    UserAccount = "firefly",
                    UserRole = UserConstant.ADMIN_ROLE,
                    UserPassword = "0732d07230a94c9318ecdfc223dfb310" // 12345678
                };
                context.Users.Add(defaultUser);
                context.SaveChanges();
            }
        }
    }



}
