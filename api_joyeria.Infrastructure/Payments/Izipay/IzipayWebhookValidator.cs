using System.Text.Json;
using Microsoft.Extensions.Options;

namespace api_joyeria.Infrastructure.Payments.Izipay
{
    // Si prefieres separar validación, usa esta clase; aquí delegamos a IzipayPaymentGateway.ValidateNotificationAsync
    public class IzipayWebhookValidator
    {
        private readonly IzipayOptions _opts;
        public IzipayWebhookValidator(IOptions<IzipayOptions> options)
        {
            _opts = options.Value;
        }

        public bool IsValid(JsonElement payload, string signature)
        {
            // Helper local: serializa payload de forma consistente y calcula HMAC
            var payloadString = JsonSerializer.Serialize(payload);
            var expectedHex = ComputeHmacSha256Hex(_opts.ApiSecret, payloadString);
            return !string.IsNullOrEmpty(signature) && string.Equals(signature, expectedHex, StringComparison.OrdinalIgnoreCase);
        }

        private static string ComputeHmacSha256Hex(string secret, string message)
        {
            var key = System.Text.Encoding.UTF8.GetBytes(secret ?? "");
            var bytes = System.Text.Encoding.UTF8.GetBytes(message ?? "");
            using var hmac = new System.Security.Cryptography.HMACSHA256(key);
            var hash = hmac.ComputeHash(bytes);
            var sb = new System.Text.StringBuilder(hash.Length * 2);
            foreach (var b in hash) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}