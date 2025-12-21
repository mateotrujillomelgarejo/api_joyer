using System.Threading;
using System.Threading.Tasks;
using api_joyeria.Domain.Entities;

namespace api_joyeria.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order, CancellationToken cancellationToken = default);
        Task<Order> GetByIdAsync(string orderId, CancellationToken cancellationToken = default);
        Task UpdateAsync(Order order, CancellationToken cancellationToken = default);
    }
}