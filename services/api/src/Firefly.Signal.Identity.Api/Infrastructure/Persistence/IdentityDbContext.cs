using Firefly.Signal.Identity.Domain;
using Firefly.Signal.SharedKernel.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.Identity.Infrastructure.Persistence;

public sealed class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    public const string SchemaName = "identity";

    public DbSet<UserAccount> Users => Set<UserAccount>();

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

        modelBuilder.ApplySoftDeleteQueryFilters();
    }
}
