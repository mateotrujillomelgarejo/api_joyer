using api_joyeria.Domain.Entities;

namespace api_joyeria.Application.Interfaces.Repositories;

public interface ICartRepository : IRepository<Cart>
{
    Task<IEnumerable<Cart>> GetActiveCartsAsync(CancellationToken ct = default);
    Task<Cart?> GetCartByTokenAsync(string guestToken, CancellationToken ct = default);
    Task ExpireCartAsync(int cartId, CancellationToken ct = default);
    Task<IEnumerable<Cart>> GetExpiredCartsAsync(DateTime expiryDate, CancellationToken ct = default);
    Task DeleteExpiredAsync(DateTime now, CancellationToken ct = default);
}