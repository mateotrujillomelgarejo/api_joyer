using api_joyeria.Domain.Entities;

namespace api_joyeria.Infrastructure.Repositories;

public interface IPaymentRepository
{
    Task AddAsync(Payment payment, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
