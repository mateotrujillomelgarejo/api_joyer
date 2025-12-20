using System.Threading;
using System.Threading.Tasks;

namespace api_joyeria.Application.Interfaces;

// Adapter interface implemented in Infrastructure for real gateway (Stripe/PayPal...)
public interface IPaymentGateway
{
    // Crea un "payment intent" o inicia el pago y devuelve información útil para frontend (client secret, gateway id, si requiere confirmación).
    Task<PaymentGatewayResult> CreatePaymentIntentAsync(int orderId, decimal amount, string method, string idempotencyKey, CancellationToken ct = default);

    // Verifica firma/payload del webhook. Implementación concreta conoce algoritmo de firma del proveedor.
    bool VerifyWebhookSignature(string payload, string signatureHeader, string webhookSecret);

    // Parsear el payload del webhook y devolver un evento estándar (gatewayId, status, metadata).
    GatewayWebhookEvent ParseWebhookEvent(string payload);
}

public record PaymentGatewayResult(string GatewayId, string ClientSecret, bool RequiresConfirmation, string RawResponse);

public record GatewayWebhookEvent(string GatewayId, string Status, decimal? Amount = null, string? RawPayload = null);