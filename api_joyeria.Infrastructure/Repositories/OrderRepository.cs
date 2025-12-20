using api_joyeria.Domain.Entities;
using api_joyeria.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using api_joyeria.Application.Interfaces.Repositories;

namespace api_joyeria.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _ctx;
    public OrderRepository(ApplicationDbContext ctx) => _ctx = ctx;

    public async Task AddAsync(Order order, CancellationToken ct = default)
    {
        await _ctx.Orders.AddAsync(order, ct);
    }

    public async Task<Order?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _ctx.Orders
            .Include(o => o.Items)
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

    public void Update(Order order) => _ctx.Orders.Update(order);

    public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        _ctx.SaveChangesAsync(ct);
}