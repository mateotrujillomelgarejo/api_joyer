using AutoMapper;
using api_joyeria.Application.DTOs;
using api_joyeria.Application.Interfaces;
using api_joyeria.Domain.Entities;

namespace api_joyeria.Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepo;
    private readonly IMapper _mapper;

    public CartService(ICartRepository cartRepo, IMapper mapper)
    {
        _cartRepo = cartRepo;
        _mapper = mapper;
    }

    public async Task<CartDto> CreateCartAsync()
    {
        var cart = new Cart
        {
            GuestToken = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.UtcNow,
            ExpiredAt = DateTime.UtcNow.AddMinutes(30) // Expira en 30 minutos
        };

        await _cartRepo.AddAsync(cart);
        await _cartRepo.SaveChangesAsync();
        return _mapper.Map<CartDto>(cart);
    }

    public async Task<CartDto?> GetCartByTokenAsync(string guestToken, CancellationToken ct = default)
    {
        var cart = await _cartRepo.GetCartByTokenAsync(guestToken, ct);
        return cart == null ? null : _mapper.Map<CartDto>(cart);
    }

    public async Task<IEnumerable<CartDto>> GetActiveCartsAsync(CancellationToken ct = default)
    {
        var activeCarts = await _cartRepo.GetActiveCartsAsync(ct);
        return _mapper.Map<IEnumerable<CartDto>>(activeCarts);
    }

    public async Task ExpireCartAsync(int cartId, CancellationToken ct = default)
    {
        await _cartRepo.ExpireCartAsync(cartId, ct);
    }

    public async Task<CartDto> AddItemToCartAsync(int cartId, CartItemDto itemDto)
    {
        var cart = await _cartRepo.GetByIdAsync(cartId);
        if (cart == null) throw new KeyNotFoundException($"Carrito {cartId} no encontrado.");

        if (cart.ExpiredAt <= DateTime.UtcNow)
            throw new InvalidOperationException($"El carrito {cartId} ya ha expirado.");

        var item = _mapper.Map<CartItem>(itemDto);
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == itemDto.ProductId);

        if (existingItem != null)
        {
            existingItem.Quantity += item.Quantity;
        }
        else
        {
            cart.Items.Add(item);
        }

        await _cartRepo.SaveChangesAsync();
        return _mapper.Map<CartDto>(cart);
    }

    public async Task<CartDto?> GetCartByIdAsync(int cartId, CancellationToken ct = default)
    {
        var cart = await _cartRepo.GetByIdAsync(cartId, ct);
        return cart == null ? null : _mapper.Map<CartDto>(cart);
    }


    public async Task RemoveItemFromCartAsync(int cartId, int itemId, CancellationToken ct = default)
    {
        var cart = await _cartRepo.GetByIdAsync(cartId, ct);
        if (cart == null)
            throw new KeyNotFoundException("Carrito no encontrado");

        var item = cart.Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            throw new KeyNotFoundException("Producto no encontrado");

        cart.Items.Remove(item);
        await _cartRepo.SaveChangesAsync(ct);
    }


    public async Task ClearCartAsync(int cartId, CancellationToken ct = default)
    {
        var cart = await _cartRepo.GetByIdAsync(cartId, ct);
        if (cart == null)
            throw new KeyNotFoundException("Carrito no encontrado");

        cart.Items.Clear();
        await _cartRepo.SaveChangesAsync(ct);
    }

}