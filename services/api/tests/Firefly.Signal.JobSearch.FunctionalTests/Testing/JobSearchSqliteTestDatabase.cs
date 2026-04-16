using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.JobSearch.FunctionalTests.Testing;

internal sealed class JobSearchSqliteTestDatabase : IDisposable, IAsyncDisposable
{
    private readonly SqliteConnection connection;
    private readonly DbContextOptions<JobSearchDbContext> options;

    public JobSearchSqliteTestDatabase()
    {
        connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        options = new DbContextOptionsBuilder<JobSearchDbContext>()
            .UseSqlite(connection)
            .Options;

        using var context = CreateDbContext();
        context.Database.EnsureCreated();
    }

    public JobSearchDbContext CreateDbContext()
        => new(options);

    public void Dispose()
        => connection.Dispose();

    public ValueTask DisposeAsync()
        => connection.DisposeAsync();
}
