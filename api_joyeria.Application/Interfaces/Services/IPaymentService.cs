using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using api_joyeria.Application.DTOs.Payment;

namespace api_joyeria.Application.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<PaymentInitResponseDto> InitializePaymentAsync(string orderId, string returnUrl, string cancelUrl, CancellationToken cancellationToken = default);
        Task ConfirmPaymentAsync(string paymentReference, string gatewayStatus, string orderId, JsonElement payload, CancellationToken cancellationToken = default);
    }
}