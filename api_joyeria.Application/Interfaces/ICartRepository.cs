using api_joyeria.Domain.Entities;

namespace api_joyeria.Application.Interfaces;

public interface ICartRepository : IRepository<Cart>
{
    Task<IEnumerable<Cart>> GetActiveCartsAsync(CancellationToken ct = default);
    Task<Cart?> GetCartByTokenAsync(string guestToken, CancellationToken ct = default);
    Task ExpireCartAsync(Guid cartId, CancellationToken ct = default);
    Task<IEnumerable<Cart>> GetExpiredCartsAsync(DateTime expiryDate, CancellationToken ct = default);
}