using api_joyeria.Application.DTOs;
using api_joyeria.Application.Interfaces;
using api_joyeria.Application.Services;
using api_joyeria.Domain.Entities;
using api_joyeria.Infrastructure.Repositories;
using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace api_joyeria.Tests
{
    public class OrderServiceTest
    {
        [Fact]
        public async Task GuestCheckoutAsync_Should_CreateOrder_And_ReduceStock()
        {
            var product = new Producto { Id = 1, Nombre = "Anillo", Precio = 10m, Stock = 5 };
            var mockProductRepo = new Mock<IProductoRepository>();
            mockProductRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(product);
            mockProductRepo.Setup(r => r.Update(It.IsAny<Producto>()));

            var mockOrderRepo = new Mock<IOrderRepository>();
            mockOrderRepo.Setup(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                         .Returns(Task.CompletedTask);
            mockOrderRepo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                         .ReturnsAsync(1);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<GuestCheckoutDto, Order>();
                cfg.CreateMap<OrderItem, OrderItemDto>();
                cfg.CreateMap<Order, OrderDto>();
            });
            var mapper = config.CreateMapper();

            var service = new OrderService(mockProductRepo.Object, mockOrderRepo.Object, mapper);

            var dto = new GuestCheckoutDto
            {
                GuestName = "Test",
                GuestEmail = "test@example.com",
                Street = "S",
                City = "C",
                Items = new List<GuestCheckoutItemDto>
        {
            new() { ProductId = 1, Quantity = 2 }
        }
            };

            // Act
            var result = await service.GuestCheckoutAsync(dto);

            // Assert
            Assert.NotNull(result);
            mockProductRepo.Verify(r => r.Update(It.Is<Producto>(p => p.Stock == 3)), Times.Once);
            mockOrderRepo.Verify(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
