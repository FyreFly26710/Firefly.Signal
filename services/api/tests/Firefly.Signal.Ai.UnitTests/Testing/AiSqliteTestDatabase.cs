using Firefly.Signal.Ai.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.Ai.UnitTests.Testing;

internal sealed class AiSqliteTestDatabase : IDisposable, IAsyncDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<AiDbContext> _options;

    public AiSqliteTestDatabase()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<AiDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = CreateDbContext();
        context.Database.EnsureCreated();
    }

    public AiDbContext CreateDbContext() => new(_options);

    public void Dispose() => _connection.Dispose();

    public ValueTask DisposeAsync() => _connection.DisposeAsync();
}
