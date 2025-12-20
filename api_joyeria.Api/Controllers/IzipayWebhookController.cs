// api_joyeria.Api/Controllers/IzipayWebhookController.cs
using api_joyeria.Application.Interfaces;
using api_joyeria.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace api_joyeria.Api.Controllers;

[ApiController]
[Route("api/payments/izipay")]
public class IzipayWebhookController : ControllerBase
{
    private readonly IPaymentRepository _paymentRepo;
    private readonly IOrderRepository _orderRepo;
    private readonly IUnitOfWork _uow;
    private readonly IConfiguration _config;
    private readonly ILogger<IzipayWebhookController> _logger;
    private readonly IPaymentGateway _gatewayAdapter;

    public IzipayWebhookController(
        IPaymentRepository paymentRepo,
        IOrderRepository orderRepo,
        IUnitOfWork uow,
        IConfiguration config,
        ILogger<IzipayWebhookController> logger,
        IPaymentGateway gatewayAdapter)
    {
        _paymentRepo = paymentRepo;
        _orderRepo = orderRepo;
        _uow = uow;
        _config = config;
        _logger = logger;
        _gatewayAdapter = gatewayAdapter;
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook()
    {
        using var sr = new StreamReader(Request.Body);
        var body = await sr.ReadToEndAsync();

        var signatureHeader = Request.Headers["X-Izipay-Signature"].ToString();
        var webhookSecret = _config["Izipay:WebhookSecret"] ?? string.Empty;

        // Use abstraction on IPaymentGateway to verify signature
        if (!_gatewayAdapter.VerifyWebhookSignature(body, signatureHeader, webhookSecret))
        {
            _logger.LogWarning("Invalid izipay webhook signature");
            return Unauthorized();
        }

        // Parse the webhook event into normalized structure
        var ev = _gatewayAdapter.ParseWebhookEvent(body);
        var gatewayId = ev.GatewayId;
        var status = ev.Status;

        try
        {
            var payment = await _paymentRepo.FindByTransactionIdAsync(gatewayId);
            if (payment == null)
            {
                _logger.LogWarning("Payment not found for gateway id {GatewayId}", gatewayId);
                return NotFound();
            }

            // Idempotency: if already succeeded ignore
            if (payment.Status == api_joyeria.Domain.Entities.PaymentStatus.Succeeded)
                return Ok();

            var tx = await _uow.BeginTransactionAsync();
            try
            {
                if (status.Equals("succeeded", System.StringComparison.OrdinalIgnoreCase) || status.Equals("paid", System.StringComparison.OrdinalIgnoreCase))
                {
                    payment.Status = api_joyeria.Domain.Entities.PaymentStatus.Succeeded;
                    var order = await _orderRepo.GetByIdAsync(payment.OrderId);
                    order.MarkPaid();
                    _orderRepo.Update(order);
                }
                else if (status.Equals("failed", System.StringComparison.OrdinalIgnoreCase))
                {
                    payment.Status = api_joyeria.Domain.Entities.PaymentStatus.Failed;
                    var order = await _orderRepo.GetByIdAsync(payment.OrderId);
                    order.MarkPaymentFailed();
                    _orderRepo.Update(order);
                }

                _paymentRepo.Update(payment);
                await _uow.SaveChangesAsync();
                await _uow.CommitAsync(tx);
            }
            catch
            {
                await _uow.RollbackAsync(tx);
                throw;
            }

            return Ok();
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error handling izipay webhook");
            return BadRequest();
        }
    }
}