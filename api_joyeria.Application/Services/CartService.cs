using api_joyeria.Application.DTOs;
using api_joyeria.Application.Interfaces;
using api_joyeria.Domain.Entities;

namespace api_joyeria.Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly IGuestTokenGenerator _guestTokenGenerator;

    public CartService(
        ICartRepository cartRepository,
        IProductoRepository productoRepository,
        IGuestTokenGenerator guestTokenGenerator)
    {
        _cartRepository = cartRepository;
        _productoRepository = productoRepository;
        _guestTokenGenerator = guestTokenGenerator;
    }

    public async Task<CartDto> CreateCartAsync()
    {
        var gt = _guestTokenGenerator.Generate();
        var cart = new Cart
        {
            GuestToken = gt.Token,
            CreatedAt = DateTime.UtcNow,
            ExpiredAt = gt.ExpirationDate,
            Items = new List<CartItem>()
        };

        await _cartRepository.AddAsync(cart);
        await _cartRepository.SaveChangesAsync();

        return new CartDto
        {
            Id = cart.Id,
            GuestToken = cart.GuestToken,
            CreatedAt = cart.CreatedAt,
            ExpiredAt = cart.ExpiredAt,
            Items = new List<CartItemDto>()
        };
    }

    public async Task<CartDto?> GetCartByIdAsync(int cartId, CancellationToken ct = default)
    {
        var cart = await _cartRepository.GetByIdAsync(cartId, ct);
        if (cart == null) return null;
        if (cart.ExpiredAt != null && cart.ExpiredAt <= DateTime.UtcNow) return null;

        return new CartDto
        {
            Id = cart.Id,
            GuestToken = cart.GuestToken,
            CreatedAt = cart.CreatedAt,
            ExpiredAt = cart.ExpiredAt,
            Items = cart.Items.Select(i => new CartItemDto
            {
                CartId = i.CartId,
                ProductId = i.ProductId,
                ProductName = string.Empty,
                UnitPrice = i.Price,
                Quantity = i.Quantity
            }).ToList()
        };
    }

    public async Task<CartDto?> GetCartByTokenAsync(string guestToken, CancellationToken ct = default)
        => await GetCartByTokenInternal(guestToken, ct);

    private async Task<CartDto?> GetCartByTokenInternal(string guestToken, CancellationToken ct)
    {
        var cart = await _cartRepository.GetCartByTokenAsync(guestToken, ct);
        if (cart == null) return null;
        if (cart.ExpiredAt != null && cart.ExpiredAt <= DateTime.UtcNow) return null;

        return new CartDto
        {
            Id = cart.Id,
            GuestToken = cart.GuestToken,
            CreatedAt = cart.CreatedAt,
            ExpiredAt = cart.ExpiredAt,
            Items = cart.Items.Select(i => new CartItemDto
            {
                CartId = i.CartId,
                ProductId = i.ProductId,
                ProductName = string.Empty,
                UnitPrice = i.Price,
                Quantity = i.Quantity
            }).ToList()
        };
    }

    public async Task<IEnumerable<CartDto>> GetActiveCartsAsync(CancellationToken ct = default)
    {
        var carts = await _cartRepository.GetActiveCartsAsync(ct);
        return carts.Select(c => new CartDto
        {
            Id = c.Id,
            GuestToken = c.GuestToken,
            CreatedAt = c.CreatedAt,
            ExpiredAt = c.ExpiredAt,
            Items = c.Items.Select(i => new CartItemDto
            {
                CartId = i.CartId,
                ProductId = i.ProductId,
                UnitPrice = i.Price,
                Quantity = i.Quantity
            }).ToList()
        });
    }

    public async Task ExpireCartAsync(int cartId, CancellationToken ct = default)
        => await _cartRepository.ExpireCartAsync(cartId, ct);

    public async Task<CartDto> AddItemToCartAsync(int cartId, CartItemDto itemDto)
    {
        var cart = await _cartRepository.GetByIdAsync(cartId);
        if (cart == null) throw new KeyNotFoundException("Cart not found");

        var product = await _productoRepository.GetByIdAsync(itemDto.ProductId);
        if (product == null) throw new KeyNotFoundException("Product not found");
        if (product.Stock < itemDto.Quantity) throw new InvalidOperationException("Stock insuficiente");

        var existing = cart.Items.FirstOrDefault(i => i.ProductId == itemDto.ProductId);
        if (existing != null)
        {
            existing.Quantity += itemDto.Quantity;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                CartId = cart.Id,
                ProductId = itemDto.ProductId,
                Quantity = itemDto.Quantity,
                Price = product.Precio
            });
        }

        _cartRepository.Update(cart);
        await _cartRepository.SaveChangesAsync();

        return await GetCartByIdAsync(cart.Id);
    }

    public async Task RemoveItemFromCartAsync(int cartId, int itemId, CancellationToken ct = default)
    {
        var cart = await _cartRepository.GetByIdAsync(cartId, ct);
        if (cart == null) throw new KeyNotFoundException("Cart not found");

        var item = cart.Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null) return;

        cart.Items.Remove(item);
        _cartRepository.Update(cart);
        await _cartRepository.SaveChangesAsync(ct);
    }

    public async Task ClearCartAsync(int cartId, CancellationToken ct = default)
    {
        var cart = await _cartRepository.GetByIdAsync(cartId, ct);
        if (cart == null) return;

        cart.Items.Clear();
        _cartRepository.Update(cart);
        await _cartRepository.SaveChangesAsync(ct);
    }

    public async Task<CartDto> ValidateCartBeforeCheckoutAsync(int cartId, CancellationToken ct = default)
    {
        var cartDto = await GetCartByIdAsync(cartId, ct);
        if (cartDto == null) throw new InvalidOperationException("Cart inválido o expirado");

        // Re-validate stock and totals server-side
        foreach (var it in cartDto.Items)
        {
            var product = await _productoRepository.GetByIdAsync(it.ProductId, ct);
            if (product == null) throw new KeyNotFoundException($"Producto {it.ProductId} no encontrado");
            if (product.Stock < it.Quantity) throw new InvalidOperationException($"Stock insuficiente para el producto {it.ProductId}");
        }

        return cartDto;
    }
}