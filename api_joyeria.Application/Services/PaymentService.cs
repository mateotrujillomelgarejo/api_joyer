using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using api_joyeria.Application.DTOs.Payment;
using api_joyeria.Application.Interfaces;
using api_joyeria.Application.Interfaces.Repositories;
using api_joyeria.Application.Interfaces.Services;
using api_joyeria.Domain.Entities;
using api_joyeria.Domain.Enums;
using api_joyeria.Domain.ValueObjects;

namespace api_joyeria.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentGateway _paymentGateway;
        private readonly IInventoryService _inventoryService;

        public PaymentService(IOrderRepository orderRepository, IPaymentRepository paymentRepository, IPaymentGateway paymentGateway, IInventoryService inventoryService)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _paymentGateway = paymentGateway ?? throw new ArgumentNullException(nameof(paymentGateway));
            _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
        }

        public async Task<PaymentInitResponseDto> InitializePaymentAsync(string orderId, string returnUrl, string cancelUrl, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
            if (order == null) throw new InvalidOperationException($"Order {orderId} not found");
            if (!order.IsPaymentAllowed()) throw new InvalidOperationException($"Order {orderId} cannot be paid in status {order.Status}");

            var req = new PaymentRequestDto
            {
                OrderId = orderId,
                Amount = order.TotalAmount.Amount,
                Currency = order.TotalAmount.Currency,
                ReturnUrl = returnUrl,
                CancelUrl = cancelUrl
            };

            var gatewayResult = await _paymentGateway.CreatePaymentAsync(req, cancellationToken);

            var payment = Payment.Create(gatewayResult.PaymentId, orderId, Money.Of(order.TotalAmount.Amount, order.TotalAmount.Currency), PaymentStatus.Pending);
            await _paymentRepository.AddAsync(payment, cancellationToken);

            return gatewayResult;
        }

        public async Task ConfirmPaymentAsync(string paymentReference, string gatewayStatus, string orderId, JsonElement payload, CancellationToken cancellationToken = default)
        {
            // Idempotencia
            var existing = await _paymentRepository.GetByReferenceAsync(paymentReference, cancellationToken);
            if (existing != null && existing.IsFinalized()) return;

            var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
            if (order == null) throw new InvalidOperationException($"Order {orderId} not found");

            // Optional: extract amount reported by gateway from payload (property names depend on gateway)
            decimal gatewayAmount = TryExtractAmount(payload);

            // Validate amount matches order total
            if (gatewayAmount > 0m && gatewayAmount != order.TotalAmount.Amount)
            {
                // suspicious notification — log and throw or mark payment rejected
                throw new InvalidOperationException($"Payment amount mismatch for order {orderId}. Gateway={gatewayAmount}, Expected={order.TotalAmount.Amount}");
            }

            bool success = string.Equals(gatewayStatus, "SUCCESS", StringComparison.OrdinalIgnoreCase)
                           || string.Equals(gatewayStatus, "APPROVED", StringComparison.OrdinalIgnoreCase);

            if (success)
            {
                if (existing == null)
                {
                    existing = Payment.Create(paymentReference, orderId, order.TotalAmount, PaymentStatus.Completed);
                    await _paymentRepository.AddAsync(existing, cancellationToken);
                }
                else
                {
                    existing.MarkAsCompleted();
                    await _paymentRepository.UpdateAsync(existing, cancellationToken);
                }

                order.MarkAsPaid();
                await _orderRepository.UpdateAsync(order, cancellationToken);

                foreach (var oi in order.Items)
                {
                    await _inventoryService.ReduceStockAsync(oi.ProductId, oi.Quantity, cancellationToken);
                }
            }
            else
            {
                if (existing == null)
                {
                    existing = Payment.Create(paymentReference, orderId, order.TotalAmount, PaymentStatus.Failed);
                    await _paymentRepository.AddAsync(existing, cancellationToken);
                }
                else
                {
                    existing.MarkAsFailed();
                    await _paymentRepository.UpdateAsync(existing, cancellationToken);
                }

                order.MarkAsPaymentFailed();
                await _orderRepository.UpdateAsync(order, cancellationToken);
            }

            static decimal TryExtractAmount(JsonElement payload)
            {
                try
                {
                    if (payload.ValueKind == JsonValueKind.Object)
                    {
                        if (payload.TryGetProperty("amount", out var a) && a.ValueKind == JsonValueKind.Number)
                            return a.GetDecimal();
                        if (payload.TryGetProperty("payment_amount", out var b) && b.ValueKind == JsonValueKind.Number)
                            return b.GetDecimal();
                    }
                }
                catch { /* ignore parse errors */ }
                return 0m;
            }
        }
    }
}