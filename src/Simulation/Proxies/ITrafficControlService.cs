namespace Simulation.Proxies;

public interface ISignalProcessService
{
    public Task SendProductionEntryAsync(Production production);
    public Task SendProductionExitAsync(Production production);
}
