using api_joyeria.Application.DTOs;
using api_joyeria.Application.Interfaces;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IGuestTokenGenerator _guestTokenGenerator;

    public 
        CartService(ICartRepository cartRepository, IGuestTokenGenerator guestTokenGenerator)
    {
        _cartRepository = cartRepository;
        _guestTokenGenerator = guestTokenGenerator;
    }

    public Cart CreateCart()
    {
        var guestToken = _guestTokenGenerator.Generate();
        var cart = new Cart
        {
            Id = Guid.NewGuid(),
            GuestToken = guestToken.Token,
            ExpirationDate = guestToken.ExpirationDate
        };
        _cartRepository.Add(cart);
        return cart;
    }

    public Cart GetCartByToken(string token)
    {
        var cart = _cartRepository.GetByToken(token);
        if (cart == null || cart.ExpirationDate < DateTime.UtcNow)
            throw new Exception("Cart not found or expired");
        return cart;
    }

    public async Task<CartDto?> GetCartByTokenAsync(string guestToken, CancellationToken ct = default)
    {
        var cart = await _cartRepository.GetByTokenAsync(guestToken, ct);

        if (cart == null || cart.ExpiryDate < DateTime.UtcNow)
            return null;

        return new CartDto
        {
            Id = cart.Id,
            GuestToken = cart.GuestToken,
            Items = cart.Items.Select(i => new CartItemDto
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList()
        };
    }

    public void AddItemToCart(string token, CartItem item)
    {
        var cart = GetCartByToken(token);
        cart.Items.Add(item);
        _cartRepository.Update(cart);
    }

    public void ClearExpiredCarts()
    {
        _cartRepository.DeleteExpired(DateTime.UtcNow);
    }
}