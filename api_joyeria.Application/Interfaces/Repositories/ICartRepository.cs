using System.Threading;
using System.Threading.Tasks;
using api_joyeria.Domain.Entities;

namespace api_joyeria.Application.Interfaces.Repositories
{
    public interface ICartRepository
    {
        Task<Cart> GetByIdAsync(string cartId, CancellationToken cancellationToken = default);
        // Opcional: Save/Update methods depend on infra design.
        Task AddAsync(Cart cart, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}