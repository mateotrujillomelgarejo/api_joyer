using api_joyeria.Application.DTOs;


namespace api_joyeria.Application.Interfaces;

public interface ICartService
{
    Task<CartDto> CreateCartAsync();
    Task<CartDto?> GetCartByIdAsync(int cartId, CancellationToken ct = default);
    Task<CartDto?> GetCartByTokenAsync(string guestToken, CancellationToken ct = default);
    Task<IEnumerable<CartDto>> GetActiveCartsAsync(CancellationToken ct = default);
    Task ExpireCartAsync(int cartId, CancellationToken ct = default);
    Task<CartDto> AddItemToCartAsync(int cartId, CartItemDto itemDto);
    Task RemoveItemFromCartAsync(int cartId, int itemId, CancellationToken ct = default);
    Task ClearCartAsync(int cartId, CancellationToken ct = default);

    Task<CartDto> ValidateCartBeforeCheckoutAsync(int cartId, CancellationToken ct = default);

}