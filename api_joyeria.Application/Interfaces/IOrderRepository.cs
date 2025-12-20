using api_joyeria.Domain.Entities;

namespace api_joyeria.Application.Interfaces;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken ct = default);
    Task<Order?> GetByIdAsync(int id, CancellationToken ct = default);
    void Update(Order order);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}