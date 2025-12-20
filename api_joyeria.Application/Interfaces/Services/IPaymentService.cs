namespace api_joyeria.Application.Interfaces.Services;

public interface IPaymentService
{
    Task<PaymentGatewayResult> ProcessPaymentAsync(int orderId, string method, string idempotencyKey, CancellationToken ct = default);

}