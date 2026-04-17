using Firefly.Signal.Ai.Domain;
using Firefly.Signal.SharedKernel.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.Ai.Infrastructure.Persistence;

public sealed class AiDbContext(DbContextOptions<AiDbContext> options) : DbContext(options)
{
    public const string SchemaName = "ai";

    public DbSet<AiMessage> AiMessages => Set<AiMessage>();
    public DbSet<AiRequest> AiRequests => Set<AiRequest>();
    public DbSet<AiResponse> AiResponses => Set<AiResponse>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName);

        modelBuilder.Entity<AiMessage>(entity =>
        {
            entity.ToTable("ai_messages");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.Type).HasConversion<string>().HasMaxLength(32).IsRequired();
            entity.Property(x => x.Content).IsRequired();
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.HasIndex(x => x.CreatedAtUtc);
        });

        modelBuilder.Entity<AiRequest>(entity =>
        {
            entity.ToTable("ai_requests");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.Source).HasConversion<string>().HasMaxLength(32).IsRequired();
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
            entity.Property(x => x.Provider).HasConversion<string>().HasMaxLength(32).IsRequired();
            entity.Property(x => x.Model).HasMaxLength(128).IsRequired();
            entity.Property(x => x.CorrelationId).HasMaxLength(256);
            entity.Property(x => x.CallerService).HasMaxLength(128);
            entity.Property(x => x.ReplyEventType).HasMaxLength(256);
            entity.Property(x => x.QueuedAtUtc).IsRequired();
            entity.Property(x => x.ProcessingStartedAtUtc);
            entity.Property(x => x.CompletedAtUtc);
            entity.Property(x => x.FailureSummary).HasMaxLength(2000);
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.Property(x => x.UpdatedAtUtc).IsRequired();
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.Source);
            entity.HasIndex(x => x.CorrelationId);
            entity.HasOne(x => x.SystemPromptMessage)
                .WithMany()
                .HasForeignKey(x => x.SystemPromptMessageId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.UserPromptMessage)
                .WithMany()
                .HasForeignKey(x => x.UserPromptMessageId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AiResponse>(entity =>
        {
            entity.ToTable("ai_responses");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.AiRequestId).IsRequired();
            entity.Property(x => x.AiResponseMessageId).IsRequired();
            entity.Property(x => x.PromptTokens);
            entity.Property(x => x.CompletionTokens);
            entity.Property(x => x.GeneratedAtUtc).IsRequired();
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.Property(x => x.UpdatedAtUtc).IsRequired();
            entity.HasIndex(x => x.AiRequestId).IsUnique();
            entity.HasOne<AiRequest>()
                .WithOne(x => x.Response)
                .HasForeignKey<AiResponse>(x => x.AiRequestId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Message)
                .WithMany()
                .HasForeignKey(x => x.AiResponseMessageId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.ApplySoftDeleteQueryFilters();
    }
}
