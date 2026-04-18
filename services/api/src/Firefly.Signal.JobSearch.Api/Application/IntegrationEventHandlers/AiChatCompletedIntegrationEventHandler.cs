using Firefly.Signal.EventBus;
using Firefly.Signal.EventBus.Events.Ai;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.JobSearch.Application.IntegrationEventHandlers;

public sealed class AiChatCompletedIntegrationEventHandler(
    JobSearchDbContext dbContext) : IIntegrationEventHandler<AiChatCompletedIntegrationEvent>
{
    public async Task HandleAsync(AiChatCompletedIntegrationEvent @event, CancellationToken ct = default)
    {
        var demoRun = await dbContext.UserJobAiChatDemoRuns
            .SingleOrDefaultAsync(run => run.CorrelationId == @event.CorrelationId, ct);

        if (demoRun is null)
            return;

        demoRun.Complete(@event.ResponseId, @event.ResponseContent);
        await dbContext.SaveChangesAsync(ct);
    }
}
