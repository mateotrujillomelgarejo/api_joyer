using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using api_joyeria.Application.Services;
using api_joyeria.Application.DTOs.Payment;
using api_joyeria.Application.Interfaces.Repositories;
using api_joyeria.Application.Interfaces;
using api_joyeria.Application.Interfaces.Services;
using api_joyeria.Domain.Entities;
using api_joyeria.Domain.ValueObjects;
using api_joyeria.Domain.Enums;
using System.Text.Json;

namespace api_joyeria.Tests.Application
{
    public class PaymentServiceTests
    {
        [Fact]
        public async Task InitializePaymentAsync_WhenOrderNotFound_ShouldThrow()
        {
            // Arrange
            var mockOrderRepo = new Mock<IOrderRepository>();
            mockOrderRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Order)null);

            var mockPaymentRepo = new Mock<IPaymentRepository>();
            var mockGateway = new Mock<IPaymentGateway>();
            var mockInventory = new Mock<IInventoryService>();

            var svc = new PaymentService(
                mockOrderRepo.Object,
                mockPaymentRepo.Object,
                mockGateway.Object,
                mockInventory.Object
            );

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => svc.InitializePaymentAsync("no-such-order", "r", "c"));
        }

        [Fact]
        public async Task InitializePaymentAsync_WhenOrderPending_ShouldCreatePaymentAndReturnGatewayResult()
        {
            // Arrange
            var order = CreatePendingOrder("ord-1", 150m, "USD");

            var mockOrderRepo = new Mock<IOrderRepository>();
            mockOrderRepo.Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>())).ReturnsAsync(order);

            var mockPaymentRepo = new Mock<IPaymentRepository>();
            var gatewayResult = new PaymentInitResponseDto { PaymentId = "pay-1", PaymentUrl = "https://pay" };
            var mockGateway = new Mock<IPaymentGateway>();
            mockGateway.Setup(g => g.CreatePaymentAsync(It.IsAny<PaymentRequestDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(gatewayResult);

            var mockInventory = new Mock<IInventoryService>();

            var svc = new PaymentService(
                mockOrderRepo.Object,
                mockPaymentRepo.Object,
                mockGateway.Object,
                mockInventory.Object
            );

            // Act
            var res = await svc.InitializePaymentAsync(order.Id, "r", "c");

            // Assert
            Assert.Equal("pay-1", res.PaymentId);
            mockPaymentRepo.Verify(p => p.AddAsync(It.Is<Payment>(pay => pay.Reference == "pay-1" && pay.OrderId == order.Id), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ConfirmPaymentAsync_OnSuccess_ShouldMarkOrderPaidAndReduceStock()
        {
            // Arrange
            var order = CreatePendingOrder("ord-2", 40m, "USD");
            var mockOrderRepo = new Mock<IOrderRepository>();
            mockOrderRepo.Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>())).ReturnsAsync(order);

            var mockPaymentRepo = new Mock<IPaymentRepository>();
            mockPaymentRepo.Setup(r => r.GetByReferenceAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Payment)null);

            var mockGateway = new Mock<IPaymentGateway>();
            var mockInventory = new Mock<IInventoryService>();

            var svc = new PaymentService(
                mockOrderRepo.Object,
                mockPaymentRepo.Object,
                mockGateway.Object,
                mockInventory.Object
            );

            // Act
            await svc.ConfirmPaymentAsync("ref-1", "SUCCESS", order.Id, JsonDocument.Parse("{}").RootElement);

            // Assert
            mockPaymentRepo.Verify(p => p.AddAsync(It.Is<Payment>(pay => pay.Reference == "ref-1" && pay.OrderId == order.Id), It.IsAny<CancellationToken>()), Times.Once);
            mockOrderRepo.Verify(o => o.UpdateAsync(It.Is<Order>(ord => ord.Status.ToString() == "Paid"), It.IsAny<CancellationToken>()), Times.Once);
            foreach (var oi in order.Items)
            {
                mockInventory.Verify(i => i.ReduceStockAsync(oi.ProductId, oi.Quantity, It.IsAny<CancellationToken>()), Times.Once);
            }
        }

        private static Order CreatePendingOrder(string id, decimal total, string currency)
        {
            var shipping = new api_joyeria.Domain.Entities.ShippingAddress("Name", "L1", null, "City", "000", "C");
            var item = new OrderItem("p1", 2, Money.Of(total / 2m, currency));
            return Order.CreateGuestOrder(id, "e@example.com", shipping, new[] { item });
        }
    }
}