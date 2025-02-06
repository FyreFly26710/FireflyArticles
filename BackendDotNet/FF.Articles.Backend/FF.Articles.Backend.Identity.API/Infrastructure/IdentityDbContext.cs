using FF.Articles.Backend.Identity.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FF.Articles.Backend.Identity.API.Infrastructure;
public class IdentityDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureUser(modelBuilder);

        //var allUsersIncludingDeleted = _context.Users.IgnoreQueryFilters().ToList();
        modelBuilder.Entity<User>().HasQueryFilter(u => u.IsDelete == 0);//only query the data that is not deleted
    }

    private void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");
            entity.HasKey(u => u.Id); // Set primary key

            entity.Property(u => u.UserAccount).HasMaxLength(255).IsRequired();
            entity.Property(u => u.UserPassword).HasMaxLength(255).IsRequired();
            entity.Property(u => u.UserName).HasMaxLength(255);
            entity.Property(u => u.UserAvatar).HasMaxLength(1024);
            entity.Property(u => u.UserProfile).HasMaxLength(512);
            entity.Property(u => u.UserRole).HasMaxLength(50).IsRequired().HasDefaultValue("user");

            entity.Property(u => u.CreateTime);
            entity.Property(u => u.UpdateTime);
            entity.Property(u => u.IsDelete).HasDefaultValue(0);  // Set default value for IsDelete (e.g., 0 means not deleted)
        });
    }

}