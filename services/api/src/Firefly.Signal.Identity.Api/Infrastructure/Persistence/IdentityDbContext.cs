using Firefly.Signal.Identity.Domain;
using Firefly.Signal.SharedKernel.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.Identity.Infrastructure.Persistence;

public sealed class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    public const string SchemaName = "identity";

    public DbSet<UserAccount> Users => Set<UserAccount>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<UserDocument> UserDocuments => Set<UserDocument>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName);

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.UserAccountName).HasMaxLength(64).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(512).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(256);
            entity.Property(x => x.DisplayName).HasMaxLength(128);
            entity.Property(x => x.Role).HasMaxLength(32).IsRequired();
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.Property(x => x.UpdatedAtUtc).IsRequired();
            entity.HasIndex(x => x.UserAccountName).IsUnique();
            entity.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.ToTable("user_profiles");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.UserAccountId).IsRequired();
            entity.Property(x => x.FullName).HasMaxLength(200);
            entity.Property(x => x.PreferredTitle).HasMaxLength(160);
            entity.Property(x => x.PrimaryLocationPostcode).HasMaxLength(16);
            entity.Property(x => x.LinkedInUrl).HasMaxLength(512);
            entity.Property(x => x.GithubUrl).HasMaxLength(512);
            entity.Property(x => x.PortfolioUrl).HasMaxLength(512);
            entity.Property(x => x.Summary).HasMaxLength(4000);
            entity.Property(x => x.SkillsText).HasMaxLength(8000);
            entity.Property(x => x.ExperienceText).HasMaxLength(8000);
            entity.Property(x => x.PreferencesJson).HasColumnType("jsonb").IsRequired();
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.Property(x => x.UpdatedAtUtc).IsRequired();
            entity.HasIndex(x => x.UserAccountId).IsUnique();
            entity.HasOne<UserAccount>()
                .WithMany()
                .HasForeignKey(x => x.UserAccountId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserDocument>(entity =>
        {
            entity.ToTable("user_documents");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.UserAccountId).IsRequired();
            entity.Property(x => x.DocumentType).HasConversion<string>().HasMaxLength(32).IsRequired();
            entity.Property(x => x.DisplayName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.OriginalFileName).HasMaxLength(260).IsRequired();
            entity.Property(x => x.StorageKey).HasMaxLength(512).IsRequired();
            entity.Property(x => x.ContentType).HasMaxLength(256).IsRequired();
            entity.Property(x => x.FileSizeBytes).IsRequired();
            entity.Property(x => x.ChecksumSha256).HasMaxLength(64).IsRequired();
            entity.Property(x => x.IsDefault).IsRequired();
            entity.Property(x => x.UploadedAtUtc).IsRequired();
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.Property(x => x.UpdatedAtUtc).IsRequired();
            entity.HasIndex(x => x.UserAccountId);
            entity.HasIndex(x => new { x.UserAccountId, x.DocumentType, x.IsDefault });
            entity.HasOne<UserAccount>()
                .WithMany()
                .HasForeignKey(x => x.UserAccountId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.ApplySoftDeleteQueryFilters();
    }
}
