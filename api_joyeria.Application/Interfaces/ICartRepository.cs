using api_joyeria.Domain.Entities;

namespace api_joyeria.Application.Interfaces;

public interface ICartRepository : IRepository<Cart>
{
    Task<IEnumerable<Cart>> GetActiveCartsAsync(CancellationToken ct = default); // Get all active carts
    Task<Cart?> GetCartByTokenAsync(string guestToken, CancellationToken ct = default); // Find a cart by GuestToken
    Task ExpireCartAsync(int cartId, CancellationToken ct = default); // Mark a cart as expired
}