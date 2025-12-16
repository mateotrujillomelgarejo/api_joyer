using api_joyeria.Domain.Entities;

namespace api_joyeria.Infrastructure.Repositories;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken ct = default);
    Task<Order?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}