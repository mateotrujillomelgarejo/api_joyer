using api_joyeria.Application.Interfaces.Repositories;
using api_joyeria.Application.Interfaces.Services;

namespace api_joyeria.Application.Services
{
    // Servicio de dominio para inventario. Operaciones que requieren persistencia se delegan a infra.
    public class InventoryService : IInventoryService
    {
        private readonly IProductoRepository _productoRepository;

        public InventoryService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository ?? throw new ArgumentNullException(nameof(productoRepository));
        }

        public async Task ValidateStockAsync(string productId, int requiredQuantity, CancellationToken cancellationToken = default)
        {
            var product = await _productoRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null) throw new InvalidOperationException($"Product {productId} not found");
            if (!product.HasSufficientStock(requiredQuantity))
                throw new InvalidOperationException($"Insufficient stock for product {productId}");
        }

        public Task ReserveStockAsync(string productId, int quantity, CancellationToken cancellationToken = default)
        {
            // Reserva concreta implementada en Infrastructure (ej. marca 'reserved' o decremento temporal).
            return Task.CompletedTask;
        }

        public Task ReduceStockAsync(string productId, int quantity, CancellationToken cancellationToken = default)
        {
            // Reducción definitiva implementada en Infrastructure.
            return Task.CompletedTask;
        }
    }
}