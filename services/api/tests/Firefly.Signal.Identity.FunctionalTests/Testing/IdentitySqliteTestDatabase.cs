using Firefly.Signal.Identity.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.Identity.FunctionalTests.Testing;

internal sealed class IdentitySqliteTestDatabase : IDisposable, IAsyncDisposable
{
    private readonly SqliteConnection connection;
    private readonly DbContextOptions<IdentityDbContext> options;

    public IdentitySqliteTestDatabase()
    {
        connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseSqlite(connection)
            .Options;

        using var context = CreateDbContext();
        context.Database.EnsureCreated();
    }

    public IdentityDbContext CreateDbContext()
        => new(options);

    public void Dispose()
        => connection.Dispose();

    public ValueTask DisposeAsync()
        => connection.DisposeAsync();
}
