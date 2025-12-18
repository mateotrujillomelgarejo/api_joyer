using api_joyeria.Application.DTOs;

namespace api_joyeria.Application.Interfaces;

public interface IOrderService
{
    Task<OrderDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<OrderDto> CreateOrderFromCartAsync(int cartId, CheckoutDetailsDto details, CancellationToken ct = default);
}