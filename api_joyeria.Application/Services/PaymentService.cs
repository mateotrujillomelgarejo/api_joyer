using api_joyeria.Application.Interfaces;
using api_joyeria.Domain.Entities;
using api_joyeria.Infrastructure.Repositories;

namespace api_joyeria.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IOrderRepository _orderRepo;
    private readonly IPaymentRepository _paymentRepo;

    public PaymentService(
        IOrderRepository orderRepo,
        IPaymentRepository paymentRepo)
    {
        _orderRepo = orderRepo;
        _paymentRepo = paymentRepo;
    }

    public async Task ProcessPaymentAsync(
        int orderId,
        string method,
        CancellationToken ct = default)
    {
        var order = await _orderRepo.GetByIdAsync(orderId, ct)
            ?? throw new KeyNotFoundException("Orden no encontrada");

        if (order.Status == OrderStatus.Paid)
            throw new InvalidOperationException("Orden ya pagada");

        var payment = new Payment
        {
            OrderId = order.Id,
            Amount = order.Total,
            PaymentMethod = method,
            TransactionId = Guid.NewGuid().ToString(),
            Status = "Success"
        };

        order.Status = OrderStatus.Paid;

        await _paymentRepo.AddAsync(payment, ct);
        await _paymentRepo.SaveChangesAsync(ct);
    }
}
