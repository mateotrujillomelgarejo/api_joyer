using api_joyeria.Application.DTOs;
using api_joyeria.Application.DTOs.Checkout;
using api_joyeria.Application.Interfaces;
using api_joyeria.Application.Interfaces.Repositories;
using api_joyeria.Application.Interfaces.Services;
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
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
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
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
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
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity
            }).ToList()
        });
    }

    public async Task ExpireCartAsync(int cartId, CancellationToken ct = default)
        => await _cartRepository.ExpireCartAsync(cartId, ct);

    public async Task<CartDto> AddItemToCartAsync(string guestToken, AddCartItemRequestDto dto)
    {
        var cart = await _cartRepository.GetCartByTokenAsync(guestToken);
        if (cart == null)
            throw new KeyNotFoundException("Cart not found");

        if (cart.ExpiredAt.HasValue && cart.ExpiredAt.Value <= DateTime.UtcNow)
            throw new InvalidOperationException("Cart expirado");

        var product = await _productoRepository.GetByIdAsync(dto.ProductId);
        if (product == null)
            throw new KeyNotFoundException("Producto no existe");

        if (product.Stock < dto.Quantity)
            throw new InvalidOperationException("Stock insuficiente");

        // Use domain method to keep invariants
        cart.AddOrUpdateItem(product.Id, product.Nombre, product.UnitPrice, dto.Quantity);

        await _cartRepository.SaveChangesAsync();

        return await GetCartByTokenInternal(guestToken, default);
    }

    public async Task RemoveItemFromCartAsync(string guestToken, int itemId, CancellationToken ct = default)
    {
        var cart = await _cartRepository.GetCartByTokenAsync(guestToken, ct);
        if (cart == null) throw new KeyNotFoundException("Cart not found");

        var item = cart.Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null) return;

        cart.Items.Remove(item);
        _cartRepository.Update(cart);
        await _cartRepository.SaveChangesAsync(ct);
    }

    public async Task ClearCartAsync(string guestToken, CancellationToken ct = default)
    {
        var cart = await _cartRepository.GetCartByTokenAsync(guestToken, ct);
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

    public async Task StartCheckoutAsync(
        string guestToken,
        CheckoutStartRequestDto dto,
        CancellationToken ct = default)
    {
        // 1. Buscar carrito por guestToken
        var cart = await _cartRepository.GetCartByTokenAsync(guestToken, ct);
        if (cart == null)
            throw new KeyNotFoundException("Cart no encontrado");

        // 2. Verificar expiración
        if (cart.ExpiredAt.HasValue && cart.ExpiredAt.Value <= DateTime.UtcNow)
            throw new InvalidOperationException("Cart expirado");

        // 3. Verificar que no esté ya en checkout
        if (cart.IsInCheckout)
            throw new InvalidOperationException("Checkout ya iniciado para este carrito");

        // 4. Marcar estado de checkout
        cart.IsInCheckout = true;
        cart.CheckoutStartedAt = DateTime.UtcNow;

        // 5. Guardar datos guest (provisionales)
        if (!string.IsNullOrWhiteSpace(dto.GuestEmail))
            cart.GuestEmail = dto.GuestEmail;

        if (dto.ShippingAddress != null)
        {
            cart.ShippingAddress = new ShippingAddress
            {
                Street = dto.ShippingAddress.Street,
                City = dto.ShippingAddress.City,
                State = dto.ShippingAddress.State,
                ZipCode = dto.ShippingAddress.ZipCode,
                Country = dto.ShippingAddress.Country,
                Phone = dto.ShippingAddress.Phone
            };
        }

        // 6. (Opcional) Hook para reservas de stock
        // ReserveStock(cart.Items);

        // 7. Persistir todo en una sola operación
        await _cartRepository.SaveChangesAsync(ct);
    }
}