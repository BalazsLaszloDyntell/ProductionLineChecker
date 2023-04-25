namespace Simulation.Proxies;

public interface IProductionControlService
{
    public Task SendProductionEntryAsync(Production production);
    public Task SendProductionExitAsync(Production production);
}
