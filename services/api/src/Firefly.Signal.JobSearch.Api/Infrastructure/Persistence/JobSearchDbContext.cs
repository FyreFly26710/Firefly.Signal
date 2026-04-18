using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.SharedKernel.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.JobSearch.Infrastructure.Persistence;

public sealed class JobSearchDbContext(DbContextOptions<JobSearchDbContext> options) : DbContext(options)
{
    public const string SchemaName = "jobsearch";

    public DbSet<JobPosting> Jobs => Set<JobPosting>();
    public DbSet<JobRefreshRun> JobRefreshRuns => Set<JobRefreshRun>();
    public DbSet<PostcodeLookup> PostcodeLookups => Set<PostcodeLookup>();
    public DbSet<UserJobState> UserJobStates => Set<UserJobState>();
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();
    public DbSet<JobApplicationStatusEntry> JobApplicationStatusEntries => Set<JobApplicationStatusEntry>();
    public DbSet<ApplicationDocumentLink> ApplicationDocumentLinks => Set<ApplicationDocumentLink>();
    public DbSet<UserJobAiInsight> UserJobAiInsights => Set<UserJobAiInsight>();
    public DbSet<AiAnalysisRun> AiAnalysisRuns => Set<AiAnalysisRun>();
    public DbSet<UserJobAiChatDemoRun> UserJobAiChatDemoRuns => Set<UserJobAiChatDemoRun>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

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
            entity.HasIndex(x => new { x.SourceName, x.SourceJobId })
                .IsUnique()
                .HasFilter("\"SourceJobId\" <> ''");
            entity.HasIndex(x => x.Postcode);
            entity.HasIndex(x => x.PostedAtUtc);
            entity.HasIndex(x => x.IsHidden);
            entity.HasIndex(x => x.SourceName);
            entity.HasOne<JobRefreshRun>()
                .WithMany()
                .HasForeignKey(x => x.JobRefreshRunId)
                .OnDelete(DeleteBehavior.SetNull);
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
        });

        modelBuilder.Entity<PostcodeLookup>(entity =>
        {
            entity.ToTable("postcode_lookups");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.Postcode).HasMaxLength(16).IsRequired();
            entity.Property(x => x.Source).HasMaxLength(64).IsRequired();
            entity.Property(x => x.LastVerifiedAtUtc).IsRequired();
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.Property(x => x.UpdatedAtUtc).IsRequired();
            entity.HasIndex(x => x.Postcode).IsUnique();
        });

        modelBuilder.Entity<UserJobState>(entity =>
        {
            entity.ToTable("user_job_states");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.UserAccountId).IsRequired();
            entity.Property(x => x.JobPostingId).IsRequired();
            entity.Property(x => x.IsSaved).IsRequired();
            entity.Property(x => x.IsHidden).IsRequired();
            entity.Property(x => x.IsApplied).IsRequired();
            entity.Property(x => x.LastUpdatedAtUtc).IsRequired();
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.Property(x => x.UpdatedAtUtc).IsRequired();
            entity.HasIndex(x => new { x.UserAccountId, x.JobPostingId }).IsUnique();
            entity.HasIndex(x => new { x.UserAccountId, x.IsSaved });
            entity.HasIndex(x => new { x.UserAccountId, x.IsHidden });
            entity.HasIndex(x => new { x.UserAccountId, x.IsApplied });
            entity.HasOne<JobPosting>()
                .WithMany()
                .HasForeignKey(x => x.JobPostingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<JobApplication>(entity =>
        {
            entity.ToTable("job_applications");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.UserAccountId).IsRequired();
            entity.Property(x => x.JobPostingId).IsRequired();
            entity.Property(x => x.Note).HasMaxLength(4000);
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.Property(x => x.UpdatedAtUtc).IsRequired();
            entity.HasIndex(x => new { x.UserAccountId, x.JobPostingId }).IsUnique();
            entity.HasOne<JobPosting>()
                .WithMany()
                .HasForeignKey(x => x.JobPostingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<JobApplicationStatusEntry>(entity =>
        {
            entity.ToTable("job_application_status_entries");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.JobApplicationId).IsRequired();
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
            entity.Property(x => x.RoundNumber);
            entity.Property(x => x.StatusAtUtc).IsRequired();
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.Property(x => x.UpdatedAtUtc).IsRequired();
            entity.HasIndex(x => x.JobApplicationId);
            entity.HasOne<JobApplication>()
                .WithMany()
                .HasForeignKey(x => x.JobApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ApplicationDocumentLink>(entity =>
        {
            entity.ToTable("application_document_links");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.JobApplicationId).IsRequired();
            entity.Property(x => x.UserDocumentId).IsRequired();
            entity.Property(x => x.LinkType).HasConversion<string>().HasMaxLength(48).IsRequired();
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.Property(x => x.UpdatedAtUtc).IsRequired();
            entity.HasIndex(x => x.JobApplicationId);
            entity.HasIndex(x => new { x.JobApplicationId, x.UserDocumentId, x.LinkType }).IsUnique();
            entity.HasOne<JobApplication>()
                .WithMany()
                .HasForeignKey(x => x.JobApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AiAnalysisRun>(entity =>
        {
            entity.ToTable("ai_analysis_runs");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.RequestedByUserAccountId).IsRequired();
            entity.Property(x => x.TargetUserAccountId).IsRequired();
            entity.Property(x => x.Mode).HasConversion<string>().HasMaxLength(32).IsRequired();
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
            entity.Property(x => x.JobCount).IsRequired();
            entity.Property(x => x.StartedAtUtc).IsRequired();
            entity.Property(x => x.CompletedAtUtc);
            entity.Property(x => x.FailureSummary).HasMaxLength(2000);
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.Property(x => x.UpdatedAtUtc).IsRequired();
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.StartedAtUtc);
        });

        modelBuilder.Entity<UserJobAiInsight>(entity =>
        {
            entity.ToTable("user_job_ai_insights");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.UserAccountId).IsRequired();
            entity.Property(x => x.JobPostingId).IsRequired();
            entity.Property(x => x.GeneratedByUserAccountId).IsRequired();
            entity.Property(x => x.AiAnalysisRunId);
            entity.Property(x => x.Rating).IsRequired();
            entity.Property(x => x.Summary).HasMaxLength(4000);
            entity.Property(x => x.DetailedExplanation).HasMaxLength(12000);
            entity.Property(x => x.CvImprovementSuggestions).HasMaxLength(12000);
            entity.Property(x => x.PromptVersion).HasMaxLength(128);
            entity.Property(x => x.GeneratedAtUtc).IsRequired();
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.Property(x => x.UpdatedAtUtc).IsRequired();
            entity.HasIndex(x => new { x.UserAccountId, x.JobPostingId, x.GeneratedAtUtc });
            entity.HasIndex(x => x.AiAnalysisRunId);
            entity.HasOne<JobPosting>()
                .WithMany()
                .HasForeignKey(x => x.JobPostingId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<AiAnalysisRun>()
                .WithMany()
                .HasForeignKey(x => x.AiAnalysisRunId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<UserJobAiChatDemoRun>(entity =>
        {
            entity.ToTable("user_job_ai_chat_demo_runs");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.UserAccountId).IsRequired();
            entity.Property(x => x.JobPostingId).IsRequired();
            entity.Property(x => x.CorrelationId).HasMaxLength(128).IsRequired();
            entity.Property(x => x.Provider).HasMaxLength(64).IsRequired();
            entity.Property(x => x.Model).HasMaxLength(128).IsRequired();
            entity.Property(x => x.Prompt).HasMaxLength(12000).IsRequired();
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
            entity.Property(x => x.AiResponseContent).HasMaxLength(12000);
            entity.Property(x => x.RequestedAtUtc).IsRequired();
            entity.Property(x => x.CompletedAtUtc);
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.Property(x => x.UpdatedAtUtc).IsRequired();
            entity.HasIndex(x => x.CorrelationId).IsUnique();
            entity.HasIndex(x => new { x.UserAccountId, x.JobPostingId, x.RequestedAtUtc });
            entity.HasOne<JobPosting>()
                .WithMany()
                .HasForeignKey(x => x.JobPostingId)
                .OnDelete(DeleteBehavior.Cascade);
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
            entity.Property(x => x.GitHubUrl).HasMaxLength(512);
            entity.Property(x => x.PortfolioUrl).HasMaxLength(512);
            entity.Property(x => x.Summary).HasMaxLength(4000);
            entity.Property(x => x.SkillsText).HasMaxLength(8000);
            entity.Property(x => x.ExperienceText).HasMaxLength(8000);
            entity.Property(x => x.PreferencesText).HasMaxLength(4000);
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.Property(x => x.UpdatedAtUtc).IsRequired();
            entity.HasIndex(x => x.UserAccountId).IsUnique();
        });

        modelBuilder.ApplySoftDeleteQueryFilters();
    }
}
