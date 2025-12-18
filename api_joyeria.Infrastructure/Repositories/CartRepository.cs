using Microsoft.EntityFrameworkCore;
using api_joyeria.Application.Interfaces;
using api_joyeria.Domain.Entities;
using api_joyeria.Infrastructure.Persistence;

namespace api_joyeria.Infrastructure.Repositories;

public class CartRepository : GenericRepository<Cart>, ICartRepository
{
    public CartRepository(ApplicationDbContext context) : base(context) { }

    // Get all active (non-expired) carts
    public async Task<IEnumerable<Cart>> GetActiveCartsAsync(CancellationToken ct = default)
    {
        return await _dbSet
            .Where(cart => cart.ExpiredAt == null || cart.ExpiredAt > DateTime.UtcNow)
            .ToListAsync(ct);
    }

    // Find a cart by token
    public async Task<Cart?> GetCartByTokenAsync(string guestToken, CancellationToken ct = default)
    {
        return await _dbSet
            .Include(cart => cart.Items) // Include items in the cart
            .FirstOrDefaultAsync(cart => cart.GuestToken == guestToken, ct);
    }

    // Mark a cart as expired
    public async Task ExpireCartAsync(int cartId, CancellationToken ct = default)
    {
        var cart = await _dbSet.FindAsync(new object[] { cartId }, ct);
        if (cart != null)
        {
            cart.ExpiredAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
        }
    }
}