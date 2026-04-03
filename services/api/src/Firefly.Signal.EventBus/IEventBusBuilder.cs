using Microsoft.Extensions.DependencyInjection;

namespace Firefly.Signal.EventBus;

public interface IEventBusBuilder
{
    IServiceCollection Services { get; }
}
