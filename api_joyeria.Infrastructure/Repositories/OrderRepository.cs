using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using api_joyeria.Application.Interfaces.Repositories;
using api_joyeria.Domain.Entities;
using api_joyeria.Infrastructure.Persistence;

namespace api_joyeria.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _ctx;

        public OrderRepository(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
        {
            await _ctx.Orders.AddAsync(order, cancellationToken);
            // Save is handled by UnitOfWork
        }

        public async Task<Order> GetByIdAsync(string orderId, CancellationToken cancellationToken = default)
        {
            return await _ctx.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
        }

        public Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
        {
            _ctx.Orders.Update(order);
            return Task.CompletedTask;
        }
    }
}