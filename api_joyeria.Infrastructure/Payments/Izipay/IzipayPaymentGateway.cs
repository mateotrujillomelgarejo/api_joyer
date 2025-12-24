using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using api_joyeria.Application.DTOs.Payment;
using api_joyeria.Application.Interfaces;

namespace api_joyeria.Infrastructure.Payments.Izipay
{
    // Implementación que usa HttpClient (registered via IHttpClientFactory)
    public class IzipayPaymentGateway : IPaymentGateway
    {
        private readonly HttpClient _http;
        private readonly IzipayOptions _options;

        public IzipayPaymentGateway(HttpClient http, IOptions<IzipayOptions> opts)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _options = opts?.Value ?? throw new ArgumentNullException(nameof(opts));
        }

        public async Task<PaymentInitResponseDto> CreatePaymentAsync(PaymentRequestDto request, CancellationToken cancellationToken = default)
        {
            // Construye el payload según lo que requiera Izipay
            var payload = new
            {
                merchant_key = _options.ApiKey,
                order_id = request.OrderId,
                amount = request.Amount,
                currency = request.Currency,
                return_url = request.ReturnUrl,
                cancel_url = request.CancelUrl
            };

            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Calcula firma HMAC-SHA256 (hex) del body con ApiSecret y la incluye en un header
            var signature = ComputeHmacSha256Hex(_options.ApiSecret, json);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var httpReq = new HttpRequestMessage(HttpMethod.Post, "payments/init"); // Ajusta path si es distinto
            httpReq.Content = content;
            httpReq.Headers.Add("X-Signature", signature);

            var res = await _http.SendAsync(httpReq, cancellationToken);
            res.EnsureSuccessStatusCode();

            var respJson = await res.Content.ReadAsStringAsync(cancellationToken);
            // Ajusta el parseo al formato real de Izipay
            using var doc = JsonDocument.Parse(respJson);
            var root = doc.RootElement;

            // Ejemplo: asumimos { paymentId: "...", paymentUrl: "..." }
            var paymentId = root.GetProperty("paymentId").GetString()
                ?? throw new InvalidOperationException("paymentId no fue retornado por Izipay");

            var paymentUrl = root.GetProperty("paymentUrl").GetString()
                ?? throw new InvalidOperationException("paymentUrl no fue retornado por Izipay");


            return new PaymentInitResponseDto
            {
                PaymentId = paymentId,
                PaymentUrl = paymentUrl
            };
        }

        public Task<bool> ValidateNotificationAsync(string payload, string signature, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(signature)) return Task.FromResult(false);

            // Calcula HMAC del payload utilizando ApiSecret (mismo algoritmo que en CreatePaymentAsync).
            var expected = ComputeHmacSha256Hex(_options.ApiSecret, payload);

            // En algunos gateways la firma llega en Base64; aquí usamos comparación case-insensitive de hex
            var ok = string.Equals(expected, signature, StringComparison.OrdinalIgnoreCase);
            return Task.FromResult(ok);
        }

        private static string ComputeHmacSha256Hex(string secret, string message)
        {
            var key = Encoding.UTF8.GetBytes(secret ?? "");
            var bytes = Encoding.UTF8.GetBytes(message ?? "");
            using var hmac = new HMACSHA256(key);
            var hash = hmac.ComputeHash(bytes);
            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}