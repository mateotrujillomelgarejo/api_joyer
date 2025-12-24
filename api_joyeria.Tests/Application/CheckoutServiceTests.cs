using api_joyeria.Application.Commands.Checkout;
using api_joyeria.Application.DTOs;
using api_joyeria.Application.DTOs.Checkout;
using api_joyeria.Application.Interfaces.Repositories;
using api_joyeria.Application.Interfaces.Services;
using api_joyeria.Application.Services;
using api_joyeria.Domain.Entities;
using api_joyeria.Domain.ValueObjects;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace api_joyeria.Tests.Application
{
    public class CheckoutServiceTests
    {
        [Fact]
        public async Task CreateGuestOrderAsync_WithCartItems_ShouldCreateOrderAndReserveStock()
        {
            // Arrange
            var cartId = "cart-1";
            var productId = "prod-1";
            var cart = new Cart(cartId);
            // FIX: use the same cartId when creating the CartItem so the domain invariant holds
            cart.AddItem(new CartItem(
                cartId,
                productId,
                2
            ));


            var mockCartRepo = new Mock<ICartRepository>();
            mockCartRepo.Setup(r => r.GetByIdAsync(cartId, It.IsAny<CancellationToken>())).ReturnsAsync(cart);

            var producto = new Producto(productId, "Ring", "Gold ring", Money.Of(50m, "USD"), stock: 10);
            var mockProductoRepo = new Mock<IProductoRepository>();
            mockProductoRepo.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync(producto);

            var mockOrderRepo = new Mock<IOrderRepository>();
            var mockInventory = new Mock<IInventoryService>();

            var service = new CheckoutService(
                mockCartRepo.Object,
                mockProductoRepo.Object,
                mockOrderRepo.Object,
                mockInventory.Object
            );

            var command = new CreateGuestOrderCommand
            {
                CartId = cartId,
                Email = "guest@example.com",
                ShippingAddress = new AddressDto { RecipientName = "John", Line1 = "L1", City = "City", PostalCode = "000", Country = "C" }
            };

            // Act
            var res = await service.CreateGuestOrderAsync(command, CancellationToken.None);

            // Assert
            Assert.NotNull(res);
            Assert.False(string.IsNullOrWhiteSpace(res.OrderId));
            Assert.Equal(100m, res.Total); // 2 * 50
            mockInventory.Verify(i => i.ValidateStockAsync(productId, 2, It.IsAny<CancellationToken>()), Times.Once);
            mockInventory.Verify(i => i.ReserveStockAsync(productId, 2, It.IsAny<CancellationToken>()), Times.Once);
            mockOrderRepo.Verify(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateGuestOrderAsync_WithItemsInCommand_ShouldCreateOrder()
        {
            // Arrange
            var productId = "prod-2";
            var mockCartRepo = new Mock<ICartRepository>(); // no cart
            mockCartRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Cart)null);

            var producto = new Producto(productId, "Necklace", "Silver", Money.Of(20m, "USD"), stock: 5);
            var mockProductoRepo = new Mock<IProductoRepository>();
            mockProductoRepo.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync(producto);

            var mockOrderRepo = new Mock<IOrderRepository>();
            var mockInventory = new Mock<IInventoryService>();

            var service = new CheckoutService(
                mockCartRepo.Object,
                mockProductoRepo.Object,
                mockOrderRepo.Object,
                mockInventory.Object
            );

            var command = new CreateGuestOrderCommand
            {
                Email = "guest2@example.com",
                Items = new List<api_joyeria.Application.DTOs.Checkout.CreateGuestOrderItemDto>
                {
                    new api_joyeria.Application.DTOs.Checkout.CreateGuestOrderItemDto { ProductId = productId, Quantity = 3 }
                },
                ShippingAddress = new AddressDto { RecipientName = "Jane", Line1 = "L1", City = "City", PostalCode = "000", Country = "C" }
            };

            // Act
            var res = await service.CreateGuestOrderAsync(command, CancellationToken.None);

            // Assert
            Assert.NotNull(res);
            Assert.Equal(60m, res.Total); // 3 * 20
            mockInventory.Verify(i => i.ValidateStockAsync(productId, 3, It.IsAny<CancellationToken>()), Times.Once);
            mockOrderRepo.Verify(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
            mockInventory.Verify(i => i.ReserveStockAsync(productId, 3, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}