using AutoMapper;
using api_joyeria.Application.DTOs;
using api_joyeria.Application.Interfaces;
using api_joyeria.Domain.Entities;
using api_joyeria.Infrastructure.Repositories;

namespace api_joyeria.Application.Services;

public class OrderService : IOrderService
{
    private readonly IProductoRepository _productRepo;
    private readonly IOrderRepository _orderRepo;
    private readonly IMapper _mapper;

    public OrderService(IProductoRepository productRepo, IOrderRepository orderRepo, IMapper mapper)
    {
        _productRepo = productRepo;
        _orderRepo = orderRepo;
        _mapper = mapper;
    }

    public async Task<OrderDto> GuestCheckoutAsync(GuestCheckoutDto dto, CancellationToken ct = default)
    {
        if (dto.Items == null || !dto.Items.Any())
            throw new ArgumentException("La orden debe contener al menos un producto.");

        // Build order
        var order = new Order
        {
            GuestName = dto.GuestName,
            GuestEmail = dto.GuestEmail,
            Street = dto.Street,
            City = dto.City,
            State = dto.State,
            Zip = dto.Zip,
            CreatedAt = DateTime.UtcNow
        };

        decimal total = 0m;

        foreach (var item in dto.Items)
        {
            // Validate product exists
            var product = await _productRepo.GetByIdAsync(item.ProductId, ct) as api_joyeria.Domain.Entities.Producto;
            if (product == null)
                throw new KeyNotFoundException($"Producto {item.ProductId} no encontrado.");

            if (product.Stock < item.Quantity)
                throw new InvalidOperationException($"Stock insuficiente para el producto '{product.Nombre}'. Disponible: {product.Stock}, solicitado: {item.Quantity}");

            decimal unitPrice = product.Precio;
            decimal subtotal = unitPrice * item.Quantity;

            // Reduce stock
            product.Stock -= item.Quantity;
            _productRepo.Update(product);

            // Add order item
            var orderItem = new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Nombre,
                UnitPrice = unitPrice,
                Quantity = item.Quantity,
                Subtotal = subtotal
            };
            order.Items.Add(orderItem);

            total += subtotal;
        }

        order.Total = total;

        // Persist
        await _orderRepo.AddAsync(order, ct);
        // Save both product stock changes and order in one SaveChanges call
        await _orderRepo.SaveChangesAsync(ct);

        // Map to DTO and return
        var orderDto = _mapper.Map<OrderDto>(order);
        return orderDto;
    }

    public async Task<OrderDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var order = await _orderRepo.GetByIdAsync(id, ct);
        return order is null ? null : _mapper.Map<OrderDto>(order);
    }
}