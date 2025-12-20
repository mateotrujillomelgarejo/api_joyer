using api_joyeria.Application.Interfaces;
using api_joyeria.Application.Interfaces.Repositories;
using api_joyeria.Application.Interfaces.Services;
using api_joyeria.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace api_joyeria.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IOrderRepository _orderRepo;
    private readonly IPaymentRepository _paymentRepo;
    private readonly IPaymentGateway _gateway;
    private readonly IUnitOfWork _uow;

    public PaymentService(
        IOrderRepository orderRepo,
        IPaymentRepository paymentRepo,
        IPaymentGateway gateway,
        IUnitOfWork uow)
    {
        _orderRepo = orderRepo;
        _paymentRepo = paymentRepo;
        _gateway = gateway;
        _uow = uow;
    }

    // Starts or processes a payment. Requires idempotencyKey from caller (header).
    public async Task<PaymentGatewayResult> ProcessPaymentAsync(int orderId, string method, string idempotencyKey, CancellationToken ct = default)
    {
        var order = await _orderRepo.GetByIdAsync(orderId, ct)
            ?? throw new KeyNotFoundException("Order not found");

        // Check existing succeeded payment
        if (order.Payments.Any(p => p.Status == PaymentStatus.Succeeded))
            return new PaymentGatewayResult(string.Empty, string.Empty, false, "{\"message\":\"already_paid\"}");

        // Check if there's an existing attempt with same idempotency key
        var existing = order.Payments.FirstOrDefault(p => p.IdempotencyKey == idempotencyKey);
        if (existing != null)
        {
            if (existing.Status == PaymentStatus.Succeeded)
                return new PaymentGatewayResult(existing.TransactionId, string.Empty, false, existing.GatewayResponse ?? string.Empty);
            // else continue and possibly return stored gateway client info if present
            if (!string.IsNullOrWhiteSpace(existing.TransactionId) && !string.IsNullOrWhiteSpace(existing.GatewayResponse))
            {
                return new PaymentGatewayResult(existing.TransactionId, string.Empty, true, existing.GatewayResponse);
            }
        }

        // Create pending payment attempt and save inside transaction
        var tx = await _uow.BeginTransactionAsync(ct);
        try
        {
            var payment = new Payment
            {
                OrderId = order.Id,
                Amount = order.Total,
                PaymentMethod = method,
                IdempotencyKey = idempotencyKey,
                Status = PaymentStatus.Pending,
                AttemptedAt = DateTime.UtcNow
            };

            await _paymentRepo.AddAsync(payment, ct);
            await _uow.SaveChangesAsync(ct);

            // Call gateway to create payment intent / process
            var result = await _gateway.CreatePaymentIntentAsync(order.Id, payment.Amount, method, idempotencyKey, ct);

            // Persist gateway info
            payment.TransactionId = result.GatewayId;
            payment.GatewayResponse = result.RawResponse;

            // If gateway returns immediate success (rare), mark succeeded
            if (!result.RequiresConfirmation)
            {
                payment.Status = PaymentStatus.Succeeded;
                order.MarkPaid();
            }

            _paymentRepo.Update(payment);
            _orderRepo.Update(order);

            await _uow.SaveChangesAsync(ct);
            await _uow.CommitAsync(tx, ct);

            return result;
        }
        catch
        {
            await _uow.RollbackAsync(tx, ct);
            throw;
        }
    }
}