using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using api_joyeria.Application.Interfaces;
using api_joyeria.Application.DTOs.Payment;

namespace api_joyeria.Infrastructure.Payments.Izipay
{
    public class IzipayPaymentGateway : IPaymentGateway
    {
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly string _webhookSecret;

        public IzipayPaymentGateway(IConfiguration configuration)
        {
            _apiKey = configuration["Izipay:ApiKey"] ?? configuration["Izipay:Key"] ?? "demo_key";
            _apiSecret = configuration["Izipay:ApiSecret"] ?? configuration["Izipay:Secret"] ?? "demo_secret";
            _webhookSecret = configuration["Izipay:WebhookSecret"] ?? "demo_webhook_secret";
        }

        public Task<PaymentInitResponseDto> CreatePaymentAsync(PaymentRequestDto request, CancellationToken cancellationToken = default)
        {
            var paymentId = $"IZP-{Guid.NewGuid():N}";
            var paymentUrl = $"https://sandbox.izipay.example/pay/{paymentId}?amount={request.Amount}";

            var res = new PaymentInitResponseDto
            {
                PaymentId = paymentId,
                PaymentUrl = paymentUrl
            };
            return Task.FromResult(res);
        }

        public Task<bool> ValidateNotificationAsync(string payload, string signature, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(signature)) return Task.FromResult(false);

            try
            {
                var secret = Encoding.UTF8.GetBytes(_webhookSecret);
                using var hmac = new HMACSHA256(secret);
                var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
                var computedHex = BitConverter.ToString(computed).Replace("-", "").ToLowerInvariant();

                var incoming = signature.Trim().ToLowerInvariant();
                return Task.FromResult(incoming == computedHex);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }
    }
}