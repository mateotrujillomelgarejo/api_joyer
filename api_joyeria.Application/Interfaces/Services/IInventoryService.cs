using System.Threading;
using System.Threading.Tasks;

namespace api_joyeria.Application.Interfaces.Services
{
    public interface IInventoryService
    {
        Task ValidateStockAsync(string productId, int requiredQuantity, CancellationToken cancellationToken = default);
        Task ReserveStockAsync(string productId, int quantity, CancellationToken cancellationToken = default);
        Task ReduceStockAsync(string productId, int quantity, CancellationToken cancellationToken = default);
    }
}