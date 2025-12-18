using api_joyeria.Application.Interfaces;

namespace api_joyeria.Application.Services;

public class CartCleanupService
{
    private readonly ICartRepository _cartRepository;

    public CartCleanupService(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task CleanExpiredCartsAsync()
    {
        await _cartRepository.DeleteExpiredAsync(DateTime.UtcNow);
    }
}