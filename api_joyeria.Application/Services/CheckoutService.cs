using api_joyeria.Application.DTOs.Checkout;
using api_joyeria.Application.Interfaces.Repositories;
using api_joyeria.Application.Interfaces.Services;
using api_joyeria.Domain.Entities;

namespace api_joyeria.Application.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IInventoryService _inventoryService;

        public CheckoutService(
            ICartRepository cartRepository,
            IProductoRepository productoRepository,
            IOrderRepository orderRepository,
            IInventoryService inventoryService)
        {
            _cartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));
            _productoRepository = productoRepository ?? throw new ArgumentNullException(nameof(productoRepository));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
        }

        public async Task<CheckoutResponseDto> CreateGuestOrderAsync(Commands.Checkout.CreateGuestOrderCommand command, CancellationToken cancellationToken = default)
        {
            // 1) Recolectar items desde cart o desde el comando
            List<(string productId, int qty)> items;

            if (!string.IsNullOrWhiteSpace(command.CartId))
            {
                var cart = await _cartRepository.GetByIdAsync(command.CartId, cancellationToken);
                if (cart == null)
                    throw new InvalidOperationException($"Cart {command.CartId} not found");

                items = cart.Items
                            .Select(i => (i.ProductId, i.Quantity))
                            .ToList();
            }
            else if (command.Items != null && command.Items.Any())
            {
                items = command.Items
                               .Select(i => (i.ProductId, i.Quantity))
                               .ToList();
            }
            else
            {
                throw new InvalidOperationException("No items provided to create order");
            }

            // 2) Validar stock
            foreach (var (productId, qty) in items)
            {
                await _inventory_service_validate(productId, qty, cancellationToken);
            }

            // 3) Construir OrderItems usando precios del domain Producto
            var orderItems = new List<OrderItem>();
            foreach (var (productId, qty) in items)
            {
                var producto = await _productoRepository.GetByIdAsync(productId, cancellationToken);
                if (producto == null) throw new InvalidOperationException($"Product {productId} not found");

                var orderItem = new OrderItem(productId, qty, producto.Price);
                orderItems.Add(orderItem);
            }

            // 4) ShippingAddress domain
            var shipping = new ShippingAddress(
                command.ShippingAddress?.RecipientName,
                command.ShippingAddress?.Line1,
                command.ShippingAddress?.Line2,
                command.ShippingAddress?.City,
                command.ShippingAddress?.PostalCode,
                command.ShippingAddress?.Country
            );

            // 5) Crear Order en domain
            var orderId = Guid.NewGuid().ToString("N");
            var order = Order.CreateGuestOrder(orderId, command.Email, shipping, orderItems);

            // 6) Persistir orden
            await _orderRepository.AddAsync(order, cancellationToken);

            // 7) Reservar stock (implementación concreta en InventoryService/Infrastructure)
            foreach (var (productId, qty) in items)
            {
                await _inventoryService.ReserveStockAsync(productId, qty, cancellationToken);
            }

            return new CheckoutResponseDto
            {
                OrderId = order.Id,
                Total = order.TotalAmount.Amount,
                Status = order.Status.ToString()
            };

            // Local wrapper to call interface method in single place (readability)
            Task _inventory_service_validate(string pid, int q, CancellationToken ct) => _inventoryService.ValidateStockAsync(pid, q, ct);
        }
    }
}