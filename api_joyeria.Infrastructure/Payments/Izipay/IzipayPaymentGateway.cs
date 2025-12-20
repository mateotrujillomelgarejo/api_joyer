// api_joyeria.Infrastructure/Payments/IzipayPaymentGateway.cs
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using api_joyeria.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace api_joyeria.Infrastructure.Payments.Izipay;

public class IzipayPaymentGateway : IPaymentGateway
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private readonly ILogger<IzipayPaymentGateway> _logger;
    private readonly string _apiKey;
    private readonly string _apiSecret;
    private readonly string _baseUrl;

    public IzipayPaymentGateway(HttpClient http, IConfiguration config, ILogger<IzipayPaymentGateway> logger)
    {
        _http = http;
        _config = config;
        _logger = logger;

        _baseUrl = _config["Izipay:BaseUrl"] ?? throw new ArgumentNullException("Izipay:BaseUrl");
        _apiKey = _config["Izipay:ApiKey"] ?? string.Empty;
        _apiSecret = _config["Izipay:ApiSecret"] ?? string.Empty;

        // optional: set default headers
        if (!string.IsNullOrEmpty(_apiKey))
            _http.DefaultRequestHeaders.Add("X-API-KEY", _apiKey);
    }

    public async Task<PaymentGatewayResult> CreatePaymentIntentAsync(int orderId, decimal amount, string method, string idempotencyKey, CancellationToken ct = default)
    {
        // Build request body according to izipay docs
        var payload = new
        {
            amount = Convert.ToInt64(amount * 100), // if izipay expects cents
            currency = "USD", // adjust
            orderId = orderId.ToString(),
            paymentMethod = method,
            // customer, return_url, etc.
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Idempotency header
        if (!string.IsNullOrEmpty(idempotencyKey))
            content.Headers.Add("Idempotency-Key", idempotencyKey);

        // TODO: replace "/payments" with the real izipay create endpoint
        var url = new Uri(new Uri(_baseUrl), "/v1/payments");

        var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
        // If izipay expects API secret signed header, add it. Example HMAC:
        // var signature = ComputeHmacSha256(_apiSecret, json);
        // req.Headers.Add("X-Signature", signature);

        var resp = await _http.SendAsync(req, ct);
        var respBody = await resp.Content.ReadAsStringAsync(ct);

        if (!resp.IsSuccessStatusCode)
        {
            _logger.LogError("Izipay CreatePaymentIntent failed: {Status} {Body}", resp.StatusCode, respBody);
            throw new InvalidOperationException("Payment gateway error");
        }

        // Parse gateway response and extract id / client info
        // TODO: adapt to real response fields
        using var doc = JsonDocument.Parse(respBody);
        var root = doc.RootElement;

        // Example: gatewayId in root["id"], client_secret in root["client_secret"]
        var gatewayId = root.GetProperty("id").GetString() ?? Guid.NewGuid().ToString("N");
        var clientSecret = root.TryGetProperty("client_secret", out var cs) ? cs.GetString() ?? string.Empty : string.Empty;
        var requiresConfirmation = root.TryGetProperty("status", out var st) && st.GetString() == "pending_confirmation";

        return new PaymentGatewayResult(gatewayId, clientSecret, requiresConfirmation, respBody);
    }

    // Util: HMAC signature if you need to sign requests
    private static string ComputeHmacSha256(string key, string message)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        using var hmac = new HMACSHA256(keyBytes);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
        return Convert.ToHexString(hash).ToLowerInvariant(); // .NET 5+ Convert.ToHexString
    }

    // Webhook signature verification helper (example HMAC)
    public bool VerifyWebhookSignature(string payload, string signatureHeader, string webhookSecret)
    {
        if (string.IsNullOrEmpty(signatureHeader) || string.IsNullOrEmpty(webhookSecret)) return false;
        var expected = ComputeHmacSha256(webhookSecret, payload);
        // signatureHeader might have prefix, compare accordingly
        return string.Equals(expected, signatureHeader, StringComparison.OrdinalIgnoreCase);
    }

    public GatewayWebhookEvent ParseWebhookEvent(string payload)
    {
        using var doc = JsonDocument.Parse(payload);
        var root = doc.RootElement;

        // TODO: mapear propiedades reales del webhook izipay
        var gatewayId = root.GetProperty("id").GetString() ?? string.Empty;
        var status = root.GetProperty("status").GetString() ?? string.Empty;
        decimal? amount = null;
        if (root.TryGetProperty("amount", out var a) && a.ValueKind == JsonValueKind.Number)
            amount = a.GetDecimal() / 100m; // si viene en centavos

        return new GatewayWebhookEvent(gatewayId, status, amount, payload);
    }
}