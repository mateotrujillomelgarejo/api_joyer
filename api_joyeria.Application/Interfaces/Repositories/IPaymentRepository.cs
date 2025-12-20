using api_joyeria.Application.Interfaces;
using api_joyeria.Domain.Entities;

namespace api_joyeria.Application.Interfaces.Repositories;

public interface IPaymentRepository
{
    Task AddAsync(Payment payment, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    void Update(Payment payment);
    Task<Payment?> FindByTransactionIdAsync(string transactionId, CancellationToken ct = default);
}
