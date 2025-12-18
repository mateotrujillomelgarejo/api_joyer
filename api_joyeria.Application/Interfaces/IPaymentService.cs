namespace api_joyeria.Application.Interfaces;

public interface IPaymentService
{
    Task<PaymentDto> CreatePaymentAsync(PaymentDto dto, CancellationToken ct = default);
    Task<PaymentDto?> GetPaymentByIdAsync(int id, CancellationToken ct = default);
}