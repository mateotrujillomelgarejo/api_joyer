using api_joyeria.Application.DTOs;

namespace api_joyeria.Application.Interfaces;

public interface IOrderService
{
    Task<OrderDto> GuestCheckoutAsync(GuestCheckoutDto dto, CancellationToken ct = default);
    Task<OrderDto?> GetByIdAsync(int id, CancellationToken ct = default);
}