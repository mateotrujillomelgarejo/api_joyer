using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace api_joyeria.Infrastructure.Payments.Izipay
{
    // Helper/validator: valida firma de webhook. Esto es un ejemplo básico.
    public class IzipayWebhookValidator
    {
        private readonly string _secret;
        public IzipayWebhookValidator(IConfiguration configuration)
        {
            _secret = configuration["Izipay:WebhookSecret"] ?? "demo_webhook_secret";
        }

        public bool IsValid(JsonElement payload, string signature)
        {
            // En producción: compute HMAC(payload, secret) and compare to signature.
            // Aquí: simple non-empty check for demo.
            if (string.IsNullOrEmpty(signature)) return false;
            return signature.Length > 5;
        }
    }
}