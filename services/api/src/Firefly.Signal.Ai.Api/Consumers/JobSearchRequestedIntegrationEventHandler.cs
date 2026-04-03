using Firefly.Signal.EventBus;

namespace Firefly.Signal.Ai.Api.Consumers;

public sealed class JobSearchRequestedIntegrationEventHandler(
    ILogger<JobSearchRequestedIntegrationEventHandler> logger) : IIntegrationEventHandler<JobSearchRequestedIntegrationEvent>
{
    public Task HandleAsync(JobSearchRequestedIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Received job search request {SearchId} for postcode {Postcode} and keyword {Keyword} with {ResultCount} results",
            @event.SearchId,
            @event.Postcode,
            @event.Keyword,
            @event.ResultCount);

        return Task.CompletedTask;
    }
}
