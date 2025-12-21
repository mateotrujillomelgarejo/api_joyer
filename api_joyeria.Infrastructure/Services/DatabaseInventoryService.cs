using System;
using System.Threading;
using System.Threading.Tasks;
using api_joyeria.Application.Interfaces.Services;
using api_joyeria.Application.Interfaces.Repositories;
using api_joyeria.Infrastructure.Persistence;

namespace api_joyeria.Infrastructure.Services
{
    // Implementación técnica del inventario sobre la BD.
    // No contiene reglas de negocio — sólo operaciones técnicas.
    public class DatabaseInventoryService : IInventoryService
    {
        private readonly IProductoRepository _productoRepository;
        private readonly ApplicationDbContext _context;

        public DatabaseInventoryService(IProductoRepository productoRepository, ApplicationDbContext context)
        {
            _productoRepository = productoRepository ?? throw new ArgumentNullException(nameof(productoRepository));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task ValidateStockAsync(string productId, int requiredQuantity, CancellationToken cancellationToken = default)
        {
            var p = await _productoRepository.GetByIdAsync(productId, cancellationToken);
            if (p == null) throw new InvalidOperationException($"Product {productId} not found");
            if (!p.HasSufficientStock(requiredQuantity))
                throw new InvalidOperationException($"Insufficient stock for product {productId}");
        }

        public Task ReserveStockAsync(string productId, int quantity, CancellationToken cancellationToken = default)
        {
            // Example: you may create a reservation row or decrement a reserved counter.
            // Here we do nothing and leave reservation semantics to business decisions.
            return Task.CompletedTask;
        }

        public async Task ReduceStockAsync(string productId, int quantity, CancellationToken cancellationToken = default)
        {
            var product = await _productoRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null) throw new InvalidOperationException($"Product {productId} not found");

            product.ReduceStock(quantity);
            // Persist change through DbContext
            _context.Productos.Update(product);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}