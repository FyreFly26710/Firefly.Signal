using Firefly.Signal.JobSearch.Domain;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.JobSearch.Infrastructure.Persistence;

public sealed class JobSearchDbContext(DbContextOptions<JobSearchDbContext> options) : DbContext(options)
{
    public const string SchemaName = "jobsearch";

    public DbSet<JobPosting> Jobs => Set<JobPosting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName);

        modelBuilder.Entity<JobPosting>(entity =>
        {
            entity.ToTable("jobs");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Company).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Postcode).HasMaxLength(16).IsRequired();
            entity.Property(x => x.LocationName).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Summary).HasMaxLength(4000).IsRequired();
            entity.Property(x => x.Url).HasMaxLength(512).IsRequired();
            entity.Property(x => x.SourceName).HasMaxLength(64).IsRequired();
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.Property(x => x.UpdatedAtUtc).IsRequired();
            entity.HasIndex(x => x.Postcode);
            entity.HasIndex(x => x.PostedAtUtc);
            entity.HasQueryFilter(x => !x.IsDeleted);
        });
    }
}
