using api_joyeria.Application.Interfaces;
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

        public async Task AddItemAsync(string cartId, string productId, int quantity, CancellationToken cancellationToken = default)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId, cancellationToken);

            if (cart == null)
            {
                cart = new Cart(cartId);
                await _cartRepository.AddAsync(cart, cancellationToken);
            }

            var product = await _productoRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new InvalidOperationException($"Product {productId} not found");

            cart.AddItem(new CartItem(cartId, productId, quantity));

            await _cartRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task<Cart> GetCartAsync(string cartId, CancellationToken cancellationToken = default)
        {
            return await _cartRepository.GetByIdAsync(cartId, cancellationToken);
        }

        public async Task UpdateItemQuantityAsync(string cartId, string productId, int quantity, CancellationToken cancellationToken = default)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId, cancellationToken);
            if (cart == null) throw new InvalidOperationException($"Cart {cartId} not found");

            cart.UpdateItemQuantity(productId, quantity);
            // Persistencia delegada a Infrastructure
        }

        public async Task RemoveItemAsync(string cartId, string productId, CancellationToken cancellationToken = default)
        {
            var cart = await _cart_repository_get(cartId, cancellationToken);
            cart.RemoveItem(productId);
            // Persistencia delegada a Infrastructure
        }

        Task<Cart> _cart_repository_get(string id, CancellationToken ct) => _cartRepository.GetByIdAsync(id, ct);
    }
}