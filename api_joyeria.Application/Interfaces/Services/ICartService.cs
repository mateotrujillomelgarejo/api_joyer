using api_joyeria.Application.DTOs;
using api_joyeria.Application.DTOs.Checkout;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace api_joyeria.Application.Interfaces.Services;

public interface ICartService
{
    Task StartCheckoutAsync(string guestToken, CheckoutStartRequestDto dto, CancellationToken ct = default);
    Task<CartDto> CreateCartAsync();
    Task<CartDto?> GetCartByIdAsync(int cartId, CancellationToken ct = default);
    Task<CartDto?> GetCartByTokenAsync(string guestToken, CancellationToken ct = default);
    Task<IEnumerable<CartDto>> GetActiveCartsAsync(CancellationToken ct = default);
    Task ExpireCartAsync(int cartId, CancellationToken ct = default);
    Task<CartDto> AddItemToCartAsync(string guestToken, AddCartItemRequestDto dto);
    Task RemoveItemFromCartAsync(string guestToken, int itemId, CancellationToken ct = default);
    Task ClearCartAsync(string guestToken, CancellationToken ct = default);

    Task<CartDto> ValidateCartBeforeCheckoutAsync(int cartId, CancellationToken ct = default);
}