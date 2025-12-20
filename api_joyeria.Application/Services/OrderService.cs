using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using api_joyeria.Application.DTOs;
using api_joyeria.Application.Interfaces;
using api_joyeria.Application.Interfaces.Repositories;
using api_joyeria.Application.Interfaces.Services;
using api_joyeria.Domain.Entities;
using api_joyeria.Domain.Enums;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage;

namespace api_joyeria.Application.Services;

public class OrderService : IOrderService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IInventoryService _inventoryService;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public OrderService(
        ICartRepository cartRepository,
        IProductoRepository productoRepository,
        IOrderRepository orderRepository,
        IInventoryService inventoryService,
        IUnitOfWork uow,
        IMapper mapper)
    {
        _cartRepository = cartRepository;
        _productoRepository = productoRepository;
        _orderRepository = orderRepository;
        _inventoryService = inventoryService;
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<OrderDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, ct);
        return order is null ? null : _mapper.Map<OrderDto>(order);
    }

    // Now receives guestToken (string) instead of cartId
    public async Task<OrderDto> CreateOrderFromCartAsync(string guestToken, CheckoutRequestDto details, string idempotencyKey, CancellationToken ct = default)
    {
        // Load cart with items by token
        var cart = await _cartRepository.GetCartByTokenAsync(guestToken, ct)
            ?? throw new KeyNotFoundException("Cart not found");

        // Domain check
        cart.EnsureNotExpired();

        if (cart.Items == null || cart.Items.Count == 0)
            throw new InvalidOperationException("Cart is empty");

        // Prevent duplicate order creation if cart already consumed
        if (cart.IsConsumed)
            throw new InvalidOperationException("Order already created for this cart");

        // Prepare item list for inventory checks
        var itemsToCheck = cart.Items.Select(i => (productId: i.ProductId, qty: i.Quantity)).ToList();

        // Best-effort availability check
        if (!await _inventoryService.CheckAvailabilityAsync(itemsToCheck, ct))
            throw new InvalidOperationException("Insufficient stock for one or more products");

        // Begin transaction
        var tx = await _uow.BeginTransactionAsync(ct);
        try
        {
            // Reserve/Decrement stock (must be part of same transaction)
            await _inventoryService.ReserveOrDecrementAsync(itemsToCheck, ct);

            // Map cart -> order (snapshot names/prices)
            var order = new Order
            {
                CartId = cart.Id,
                GuestEmail = details.GuestEmail,
                Customer = new OrderCustomer { FullName = details.GuestName, Phone = details.Phone ?? string.Empty },
                Shipping = new ShippingAddress
                {
                    Street = details.Street,
                    City = details.City,
                    State = details.State,
                    ZipCode = details.Zip,
                    Country = details.Country,
                    Phone = details.Phone
                },
                Status = OrderStatus.Pending,
                CreatedAt = System.DateTime.UtcNow,
                Items = cart.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    SKU = string.Empty,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            order.Validate();

            await _orderRepository.AddAsync(order, ct);
            await _uow.SaveChangesAsync(ct); // persists order + product updates from inventory service

            // Mark cart consumed inside same transaction
            cart.IsConsumed = true;
            cart.ExpiredAt = System.DateTime.UtcNow;
            _cartRepository.Update(cart);
            await _uow.SaveChangesAsync(ct);

            await _uow.CommitAsync(tx, ct);

            return _mapper.Map<OrderDto>(order);
        }
        catch
        {
            await _uow.RollbackAsync(tx, ct);
            throw;
        }
    }
}