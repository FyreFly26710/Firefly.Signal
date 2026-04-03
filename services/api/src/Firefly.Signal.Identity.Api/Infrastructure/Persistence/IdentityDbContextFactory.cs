using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Firefly.Signal.Identity.Infrastructure.Persistence;

public sealed class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=firefly_signal;Username=firefly;Password=firefly",
            npgsql => npgsql.MigrationsHistoryTable(HistoryRepository.DefaultTableName, IdentityDbContext.SchemaName));
        return new IdentityDbContext(optionsBuilder.Options);
    }
}
