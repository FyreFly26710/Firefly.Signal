using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Firefly.Signal.Ai.Infrastructure.Persistence;

public sealed class AiDbContextFactory : IDesignTimeDbContextFactory<AiDbContext>
{
    public AiDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AiDbContext>();
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=firefly_signal;Username=firefly;Password=firefly",
            npgsql => npgsql.MigrationsHistoryTable(HistoryRepository.DefaultTableName, AiDbContext.SchemaName));
        return new AiDbContext(optionsBuilder.Options);
    }
}
