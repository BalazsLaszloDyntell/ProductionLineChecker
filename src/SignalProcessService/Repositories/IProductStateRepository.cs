using SignalProcessService.Models;

namespace SignalProcessService.Repositories;

public interface IProductStateRepository
{
    Task SaveProductStateAsync(ProductState productState);
    Task<ProductState?> GetProductStateAsync(string licenseNumber);
}
