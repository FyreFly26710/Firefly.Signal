using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Firefly.Signal.JobSearch.Infrastructure.Persistence;

public sealed class JobSearchDbContextFactory : IDesignTimeDbContextFactory<JobSearchDbContext>
{
    public JobSearchDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<JobSearchDbContext>();
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=firefly_signal;Username=firefly;Password=firefly",
            npgsql => npgsql.MigrationsHistoryTable(HistoryRepository.DefaultTableName, JobSearchDbContext.SchemaName));
        return new JobSearchDbContext(optionsBuilder.Options);
    }
}
