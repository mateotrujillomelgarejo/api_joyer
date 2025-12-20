using Microsoft.EntityFrameworkCore;
using api_joyeria.Application.Interfaces;
using api_joyeria.Infrastructure.Persistence;

namespace api_joyeria.Infrastructure.Repositories;

public class CartRepository : GenericRepository<Cart>, ICartRepository
{
    public CartRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Cart>> GetActiveCartsAsync(CancellationToken ct = default)
    {
        return await _dbSet
            .Where(cart => cart.ExpiredAt == null || cart.ExpiredAt > DateTime.UtcNow)
            .Include(c => c.Items)
            .ToListAsync(ct);
    }

    public async Task<Cart?> GetCartByTokenAsync(string guestToken, CancellationToken ct = default)
    {
        return await _dbSet
            .Include(cart => cart.Items)
            .FirstOrDefaultAsync(cart => cart.GuestToken == guestToken, ct);
    }

    public async Task ExpireCartAsync(int cartId, CancellationToken ct = default)
    {
        var cart = await _dbSet.FindAsync(new object[] { cartId }, ct);
        if (cart != null)
        {
            cart.ExpiredAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task<IEnumerable<Cart>> GetExpiredCartsAsync(DateTime expiryDate, CancellationToken ct = default)
    {
        return await _dbSet
            .Where(c => c.ExpiredAt != null && c.ExpiredAt <= expiryDate)
            .ToListAsync(ct);
    }

    public async Task DeleteExpiredAsync(DateTime now, CancellationToken ct = default)
    {
        var expired = await GetExpiredCartsAsync(now, ct);
        if (expired.Any())
        {
            _dbSet.RemoveRange(expired);
            await _context.SaveChangesAsync(ct);
        }
    }
}