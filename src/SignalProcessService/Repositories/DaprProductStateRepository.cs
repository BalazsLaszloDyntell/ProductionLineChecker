using Dapr.Client;
using SignalProcessService.Models;

namespace SignalProcessService.Repositories;

public class DaprProductStateRepository : IProductStateRepository
{
    private const string DAPR_STORE_NAME = "statestore";
    private readonly DaprClient _daprClient;

    public DaprProductStateRepository(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    public async Task SaveProductStateAsync(ProductState productState)
    {
        await _daprClient.SaveStateAsync<ProductState>(
            DAPR_STORE_NAME, productState.Barcode, productState);
    }

    public async Task<ProductState?> GetProductStateAsync(string barcode)
    {
        var stateEntry = await _daprClient.GetStateEntryAsync<ProductState>(
            DAPR_STORE_NAME, barcode);
        return stateEntry.Value;
    }
}
