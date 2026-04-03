namespace Firefly.Signal.EventBus;

public sealed record JobSearchRequestedIntegrationEvent(
    long SearchId,
    string Postcode,
    string Keyword,
    int ResultCount) : IntegrationEvent;
