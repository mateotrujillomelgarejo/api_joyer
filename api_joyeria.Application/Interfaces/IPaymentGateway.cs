using System.Threading;
using System.Threading.Tasks;
using api_joyeria.Application.DTOs.Payment;

namespace api_joyeria.Application.Interfaces
{
    public interface IPaymentGateway
    {
        Task<PaymentInitResponseDto> CreatePaymentAsync(PaymentRequestDto request, CancellationToken cancellationToken = default);
        Task<bool> ValidateNotificationAsync(string payload, string signature, CancellationToken cancellationToken = default);
    }
}