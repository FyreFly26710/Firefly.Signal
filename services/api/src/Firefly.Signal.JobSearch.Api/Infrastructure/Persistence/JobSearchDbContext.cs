using Firefly.Signal.JobSearch.Domain;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.JobSearch.Infrastructure.Persistence;

public sealed class JobSearchDbContext(DbContextOptions<JobSearchDbContext> options) : DbContext(options)
{
    public const string SchemaName = "jobsearch";

    public DbSet<JobPosting> Jobs => Set<JobPosting>();
    public DbSet<JobRefreshRun> JobRefreshRuns => Set<JobRefreshRun>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName);

        modelBuilder.Entity<JobPosting>(entity =>
        {
            entity.ToTable("jobs");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.JobRefreshRunId);
            entity.Property(x => x.SourceName).HasMaxLength(64).IsRequired();
            entity.Property(x => x.SourceJobId).HasMaxLength(128).IsRequired();
            entity.Property(x => x.SourceAdReference).HasMaxLength(256);
            entity.Property(x => x.Title).HasMaxLength(300).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(8000).IsRequired();
            entity.Property(x => x.Company).HasMaxLength(160).IsRequired();
            entity.Property(x => x.CompanyDisplayName).HasMaxLength(200);
            entity.Property(x => x.CompanyCanonicalName).HasMaxLength(200);
            entity.Property(x => x.Postcode).HasMaxLength(16).IsRequired();
            entity.Property(x => x.LocationName).HasMaxLength(160).IsRequired();
            entity.Property(x => x.LocationDisplayName).HasMaxLength(300);
            entity.Property(x => x.LocationAreaJson).HasColumnType("jsonb");
            entity.Property(x => x.CategoryTag).HasMaxLength(120);
            entity.Property(x => x.CategoryLabel).HasMaxLength(200);
            entity.Property(x => x.Summary).HasMaxLength(4000).IsRequired();
            entity.Property(x => x.Url).HasMaxLength(1024).IsRequired();
            entity.Property(x => x.SalaryCurrency).HasMaxLength(16);
            entity.Property(x => x.ContractTime).HasMaxLength(32);
            entity.Property(x => x.ContractType).HasMaxLength(32);
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.Property(x => x.UpdatedAtUtc).IsRequired();
            entity.Property(x => x.ImportedAtUtc).IsRequired();
            entity.Property(x => x.LastSeenAtUtc).IsRequired();
            entity.Property(x => x.RawPayloadJson).HasColumnType("jsonb").IsRequired();
            entity.HasIndex(x => x.Postcode);
            entity.HasIndex(x => x.PostedAtUtc);
            entity.HasIndex(x => x.IsHidden);
            entity.HasIndex(x => x.SourceName);
            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<JobRefreshRun>(entity =>
        {
            entity.ToTable("job_refresh_runs");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.ProviderName).HasMaxLength(64).IsRequired();
            entity.Property(x => x.CountryCode).HasMaxLength(8).IsRequired();
            entity.Property(x => x.RequestFiltersJson).HasColumnType("jsonb").IsRequired();
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
            entity.Property(x => x.FailureMessage).HasMaxLength(2000).IsRequired();
            entity.Property(x => x.StartedAtUtc).IsRequired();
            entity.Property(x => x.CompletedAtUtc);
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.Property(x => x.UpdatedAtUtc).IsRequired();
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.StartedAtUtc);
            entity.HasQueryFilter(x => !x.IsDeleted);
        });
    }
}
