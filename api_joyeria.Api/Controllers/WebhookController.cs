using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using api_joyeria.Application.Commands.Payments;
using api_joyeria.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System;

namespace api_joyeria.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IPaymentGateway _paymentGateway;
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(IMediator mediator, IPaymentGateway paymentGateway, ILogger<WebhookController> logger)
        {
            _mediator = mediator;
            _paymentGateway = paymentGateway;
            _logger = logger;
        }

        [HttpPost("izipay")]
        public async Task<IActionResult> ReceiveIzipay([FromHeader(Name = "X-Signature")] string signature, [FromBody] JsonElement payload)
        {
            var payloadString = JsonSerializer.Serialize(payload);
            var ok = await _paymentGateway.ValidateNotificationAsync(payloadString, signature);
            if (!ok)
            {
                _logger.LogWarning("Invalid webhook signature");
                return BadRequest();
            }

            // Extract fields (depends on gateway payload)
            string paymentReference = payload.GetPropertyOrDefault("paymentReference") ?? payload.GetPropertyOrDefault("payment_id");
            string orderId = payload.GetPropertyOrDefault("orderId") ?? payload.GetPropertyOrDefault("order_id");
            string gatewayStatus = payload.GetPropertyOrDefault("status") ?? payload.GetPropertyOrDefault("gateway_status");

            if (string.IsNullOrWhiteSpace(paymentReference) || string.IsNullOrWhiteSpace(orderId))
            {
                _logger.LogWarning("Webhook missing required fields");
                return BadRequest();
            }

            // Respond quickly and process in background to avoid long-running webhook blocking provider
            _ = Task.Run(async () =>
            {
                try
                {
                    var cmd = new ConfirmPaymentCommand
                    {
                        PaymentReference = paymentReference,
                        OrderId = orderId,
                        GatewayStatus = gatewayStatus,
                        Payload = payload
                    };
                    await _mediator.Send(cmd);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing webhook for order {orderId}", orderId);
                }
            });

            return Ok();
        }
    }

    internal static class JsonElementExtensions
    {
        public static string GetPropertyOrDefault(this JsonElement e, string propName)
        {
            if (e.ValueKind != JsonValueKind.Object) return null;
            if (e.TryGetProperty(propName, out var v))
            {
                return v.ValueKind switch
                {
                    JsonValueKind.String => v.GetString(),
                    JsonValueKind.Number => v.GetRawText(),
                    JsonValueKind.True => "true",
                    JsonValueKind.False => "false",
                    _ => v.GetRawText()
                };
            }
            return null;
        }
    }
}