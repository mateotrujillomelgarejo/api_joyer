using api_joyeria.Application.Interfaces.Repositories;
using api_joyeria.Domain.Entities;
using api_joyeria.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace api_joyeria.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly ApplicationDbContext _ctx;

    public PaymentRepository(ApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task AddAsync(Payment payment, CancellationToken ct = default)
    {
        await _ctx.Set<Payment>().AddAsync(payment, ct);
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _ctx.SaveChangesAsync(ct);

    public void Update(Payment payment) => _ctx.Payments.Update(payment);

    public async Task<Payment?> FindByTransactionIdAsync(string transactionId, CancellationToken ct = default)
    {
        return await _ctx.Set<Payment>().FirstOrDefaultAsync(p => p.TransactionId == transactionId, ct);
    }
}
