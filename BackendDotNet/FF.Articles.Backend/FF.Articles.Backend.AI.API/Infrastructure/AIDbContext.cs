using System;
using FF.Articles.Backend.AI.API.Models.Entities;
using FF.Articles.Backend.Common.Utils;
using Microsoft.EntityFrameworkCore;

namespace FF.Articles.Backend.AI.API.Infrastructure;

public class AIDbContext : DbContext
{
    public DbSet<Session> Sessions { get; set; }
    public DbSet<ChatRound> ChatRounds { get; set; }
    public DbSet<SystemMessage> SystemMessages { get; set; }

    public AIDbContext(DbContextOptions<AIDbContext> options)
        : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("AI");

        ConfigureSession(modelBuilder);
        ConfigureChatRound(modelBuilder);
        ConfigureSystemMessage(modelBuilder);


    }

    private void ConfigureSession(ModelBuilder modelBuilder)
    {
        EFCoreUtil.ConfigBaseEntity<Session>(modelBuilder, [BaseProperty.CreateTime, BaseProperty.UpdateTime]);

        modelBuilder.Entity<Session>(entity =>
        {
            entity.ToTable("Session");
            entity.Property(e => e.SessionName).HasMaxLength(256);
            entity.Property(e => e.UserId).IsRequired().HasDefaultValue(0L);
            entity.Property(e => e.TimeStamp).IsRequired();
            entity.HasIndex(e => e.UserId).HasDatabaseName("IX_Session_UserId");
        });
    }
    private void ConfigureChatRound(ModelBuilder modelBuilder)
    {
        EFCoreUtil.ConfigBaseEntity<ChatRound>(modelBuilder, [BaseProperty.CreateTime, BaseProperty.UpdateTime]);

        modelBuilder.Entity<ChatRound>(entity =>
        {
            entity.ToTable("ChatRound");
            entity.Property(e => e.SystemMessageId).HasDefaultValue(null);
            entity.Property(e => e.UserMessage).IsRequired().HasDefaultValue("");
            entity.Property(e => e.AssistantMessage).IsRequired().HasDefaultValue("");
            entity.Property(e => e.Model).HasMaxLength(64).IsRequired().HasDefaultValue("");
            entity.Property(e => e.Provider).HasMaxLength(64).IsRequired().HasDefaultValue("");
            entity.Property(e => e.SessionId).IsRequired().HasDefaultValue(0L);
            entity.Property(e => e.TimeTaken).IsRequired().HasDefaultValue(0);
            entity.Property(e => e.PromptTokens).IsRequired().HasDefaultValue(0);
            entity.Property(e => e.CompletionTokens).IsRequired().HasDefaultValue(0);
            entity.Property(e => e.TimeStamp).IsRequired();

            entity.HasIndex(e => e.SessionId).HasDatabaseName("IX_ChatRound_SessionId");
        });
    }
    private void ConfigureSystemMessage(ModelBuilder modelBuilder)
    {
        EFCoreUtil.ConfigBaseEntity<SystemMessage>(modelBuilder, []);

        modelBuilder.Entity<SystemMessage>(entity =>
        {
            entity.ToTable("SystemMessage");
            entity.Property(e => e.Name).HasMaxLength(64).IsRequired();
            entity.Property(e => e.Content).IsRequired();
        });
    }
}