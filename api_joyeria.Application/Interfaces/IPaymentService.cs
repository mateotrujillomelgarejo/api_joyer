using api_joyeria.Application.DTOs;

namespace api_joyeria.Application.Interfaces;

public interface IPaymentService
{
    Task ProcessPaymentAsync(int orderId, string method, CancellationToken ct = default);

}