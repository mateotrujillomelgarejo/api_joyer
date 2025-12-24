using api_joyeria.Application.DTOs.api_joyeria.Api.Dtos;
using api_joyeria.Application.Interfaces.Repositories;
using api_joyeria.Application.Interfaces.Services;
using api_joyeria.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace api_joyeria.Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductoRepository _productoRepository;

        public CartService(ICartRepository cartRepository, IProductoRepository productoRepository)
        {
            _cartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));
            _productoRepository = productoRepository ?? throw new ArgumentNullException(nameof(productoRepository));
        }

        public async Task<string> AddItemAsync(
    string? cartId,
    string productId,
    int quantity,
    CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(productId))
                throw new ArgumentException("ProductId is required");

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero");

            Cart cart;

            if (string.IsNullOrWhiteSpace(cartId))
            {
                cart = new Cart(Guid.NewGuid().ToString("N"));
                await _cartRepository.AddAsync(cart, cancellationToken);
            }
            else
            {
                cart = await _cartRepository.GetByIdAsync(cartId, cancellationToken)
                    ?? throw new InvalidOperationException("Cart not found");
            }

            var product = await _productoRepository.GetByIdAsync(productId, cancellationToken)
                ?? throw new InvalidOperationException($"Product {productId} not found");

            var item = new CartItem(cart.Id, productId, quantity);
            cart.AddItem(item);

            await _cartRepository.SaveChangesAsync(cancellationToken);

            return cart.Id;
        }


        public async Task<CartDto?> GetCartAsync(string cartId, CancellationToken cancellationToken = default)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId, cancellationToken);
            if (cart == null) return null;

            var itemsDto = new List<CartItemDto>();
            decimal subtotal = 0m;

            foreach (var item in cart.Items ?? Enumerable.Empty<CartItem>())
            {
                var product = await _productoRepository.GetByIdAsync(item.ProductId, cancellationToken);
                decimal unitPrice = 0m;
                string? imageUrl = null;

                if (product != null)
                {
                    unitPrice = product.Price?.Amount ?? 0m;
                    imageUrl = product.ImageUrl;
                }

                itemsDto.Add(new CartItemDto
                {
                    ItemId = item.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice,
                    ImageUrl = imageUrl
                });

                subtotal += unitPrice * item.Quantity;
            }

            var cartDto = new CartDto
            {
                Id = cart.Id,
                Items = itemsDto,
                Subtotal = subtotal
            };

            return cartDto;
        }

        public async Task UpdateItemQuantityAsync(string cartId, string productId, int quantity, CancellationToken cancellationToken = default)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId, cancellationToken);
            if (cart == null) throw new InvalidOperationException($"Cart {cartId} not found");

            cart.UpdateItemQuantity(productId, quantity);

            // Persistir cambios
            await _cartRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveItemAsync(string cartId, string productId, CancellationToken cancellationToken = default)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId, cancellationToken);
            if (cart == null) throw new InvalidOperationException($"Cart {cartId} not found");

            cart.RemoveItem(productId);

            // Persistir cambios
            await _cartRepository.SaveChangesAsync(cancellationToken);
        }
    }
}