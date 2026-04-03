using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.IntegrationEventLogEF;

public static class IntegrationLogExtensions
{
    public static void UseIntegrationEventLogs(this ModelBuilder builder)
    {
        builder.Entity<IntegrationEventLogEntry>(entity =>
        {
            entity.ToTable("IntegrationEventLog");
            entity.HasKey(x => x.EventId);
        });
    }
}
