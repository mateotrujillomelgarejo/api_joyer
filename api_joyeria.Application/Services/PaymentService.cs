using api_joyeria.Application.Interfaces;
using api_joyeria.Domain.Entities;
using api_joyeria.Infrastructure.Repositories;

namespace api_joyeria.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IOrderRepository _orderRepo;
    private readonly IPaymentRepository _paymentRepo;

    public PaymentService(IOrderRepository orderRepo, IPaymentRepository paymentRepo)
    {
        _orderRepo = orderRepo;
        _paymentRepo = paymentRepo;
    }

    // Procesamiento simple: en producción integrar gateway (Stripe/PayPal) y webhooks
    public async Task ProcessPaymentAsync(int orderId, string method, CancellationToken ct = default)
    {
        var order = await _orderRepo.GetByIdAsync(orderId, ct)
            ?? throw new KeyNotFoundException("Orden no encontrada");

        if (order.Status == OrderStatus.Paid)
            return; // idempotencia: ya pagada

        // Aquí iría llamada a gateway -> obtener resultado
        // Simulamos pago exitoso:
        var payment = new Payment
        {
            OrderId = order.Id,
            Amount = order.Total,
            PaymentMethod = method,
            TransactionId = Guid.NewGuid().ToString(),
            Status = "Success",
            PaidAt = DateTime.UtcNow
        };

        // Actualizar orden y persistir
        order.Status = OrderStatus.Paid;
        _orderRepo.Update(order);
        await _paymentRepo.AddAsync(payment, ct);

        // Guardar cambios (si ambos repos usan mismo DbContext, SaveChangesAsync será atómico)
        await _paymentRepo.SaveChangesAsync(ct);
        await _orderRepo.SaveChangesAsync(ct);
    }
}