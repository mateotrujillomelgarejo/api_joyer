using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using api_joyeria.Application.Interfaces.Repositories;
using api_joyeria.Domain.Entities;
using api_joyeria.Infrastructure.Persistence;

namespace api_joyeria.Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _ctx;

        public CartRepository(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task AddAsync(Cart cart, CancellationToken ct = default)
        {
            await _ctx.Carts.AddAsync(cart, ct);
        }

        public async Task<Cart> GetByIdAsync(string cartId, CancellationToken cancellationToken = default)
        {
            return await _ctx.Carts
                .Include(nameof(Cart.Items))
                .FirstOrDefaultAsync(c => c.Id == cartId, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _ctx.SaveChangesAsync(ct);
        }

    }
}