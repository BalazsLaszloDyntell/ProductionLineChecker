using Dapr.Actors;
using SignalProcessService.Events;

namespace SignalProcessService.Actors;

public interface IProductActor : IActor
{
    public Task RegisterEntryAsync(ProductRegistered msg);
    public Task RegisterExitAsync(ProductRegistered msg);
}
