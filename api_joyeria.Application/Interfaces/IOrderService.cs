using api_joyeria.Application.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace api_joyeria.Application.Interfaces;

public interface IOrderService
{
    Task<OrderDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<OrderDto> CreateOrderFromCartAsync(string guestToken, CheckoutRequestDto details, string idempotencyKey, CancellationToken ct = default);
}