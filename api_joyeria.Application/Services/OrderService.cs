using api_joyeria.Application.DTOs;
using api_joyeria.Application.Interfaces;
using api_joyeria.Domain.Entities;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace api_joyeria.Application.Services;

public class OrderService : IOrderService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public OrderService(
        ICartRepository cartRepository,
        IProductoRepository productoRepository,
        IOrderRepository orderRepository,
        IMapper mapper)
    {
        _cartRepository = cartRepository;
        _productoRepository = productoRepository;
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<OrderDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, ct);
        return order is null ? null : _mapper.Map<OrderDto>(order);
    }

    public async Task<OrderDto> CreateOrderFromCartAsync(int cartId, CheckoutDetailsDto details, CancellationToken ct = default)
    {
        var cart = await _cartRepository.GetByIdAsync(cartId, ct)
            ?? throw new KeyNotFoundException("Cart not found");

        if (cart.ExpiredAt != null && cart.ExpiredAt <= DateTime.UtcNow)
            throw new InvalidOperationException("Cart expired");

        if (cart.Items == null || cart.Items.Count == 0)
            throw new InvalidOperationException("Cart is empty");

        foreach (var item in cart.Items)
        {
            var prod = await _productoRepository.GetByIdAsync(item.ProductId, ct)
                ?? throw new KeyNotFoundException($"Producto {item.ProductId} no encontrado");

            if (prod.Stock < item.Quantity)
                throw new InvalidOperationException($"Stock insuficiente para producto {item.ProductId}");

            prod.Stock -= item.Quantity;
            _productoRepository.Update(prod);
        }

        var order = new Order
        {
            GuestEmail = details.GuestEmail,
            Total = cart.Items.Sum(i => i.Price * i.Quantity),
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            Items = cart.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList(),
            Customer = new OrderCustomer
            {
                FullName = details.GuestName,
                Phone = string.Empty
            }
        };

        await _orderRepository.AddAsync(order, ct);
        await _orderRepository.SaveChangesAsync(ct);

        cart.ExpiredAt = DateTime.UtcNow;
        _cartRepository.Update(cart);
        await _cartRepository.SaveChangesAsync(ct);

        return _mapper.Map<OrderDto>(order);
    }
}