using api_joyeria.Api.Controllers;
using api_joyeria.Api.Dtos;
using api_joyeria.Application.DTOs.api_joyeria.Api.Dtos;
using api_joyeria.Application.Interfaces.Services;
using api_joyeria.Domain.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace api_joyeria.Tests.Api
{
    public class CartControllerTests
    {
        private static CartController CreateController(
            Mock<ICartService> cartServiceMock,
            Mock<IMapper> mapperMock)
        {
            return new CartController(cartServiceMock.Object, mapperMock.Object);
        }

        [Fact]
        public async Task AddItem_NullDto_ReturnsBadRequest()
        {
            var cartServiceMock = new Mock<ICartService>();
            var mapperMock = new Mock<IMapper>();
            var controller = CreateController(cartServiceMock, mapperMock);

            var result = await controller.AddItem(null, CancellationToken.None);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task AddItem_ValidDto_CallsServiceAndReturnsNoContent()
        {
            var cartServiceMock = new Mock<ICartService>();
            cartServiceMock
                .Setup(s => s.AddItemAsync("c1", "p1", 2, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var mapperMock = new Mock<IMapper>();
            var controller = CreateController(cartServiceMock, mapperMock);

            var dto = new AddCartItemDto
            {
                CartId = "c1",
                ProductId = "p1",
                Quantity = 2
            };

            var result = await controller.AddItem(dto, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
            cartServiceMock.Verify(
                s => s.AddItemAsync("c1", "p1", 2, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task GetCart_EmptyCartId_ReturnsBadRequest()
        {
            var cartServiceMock = new Mock<ICartService>();
            var mapperMock = new Mock<IMapper>();
            var controller = CreateController(cartServiceMock, mapperMock);

            var result = await controller.GetCart("", CancellationToken.None);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task GetCart_NotFound_ReturnsNotFound()
        {
            var cartServiceMock = new Mock<ICartService>();
            cartServiceMock
                .Setup(s => s.GetCartAsync("c1", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Cart)null);

            var mapperMock = new Mock<IMapper>();
            var controller = CreateController(cartServiceMock, mapperMock);

            var result = await controller.GetCart("c1", CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetCart_Found_ReturnsOkWithCartDto()
        {
            var cart = new Cart("c1");
            var cartDto = new CartDto { Id = "c1" };

            var cartServiceMock = new Mock<ICartService>();
            cartServiceMock
                .Setup(s => s.GetCartAsync("c1", It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            var mapperMock = new Mock<IMapper>();
            mapperMock
                .Setup(m => m.Map<CartDto>(cart))
                .Returns(cartDto);

            var controller = CreateController(cartServiceMock, mapperMock);

            var result = await controller.GetCart("c1", CancellationToken.None) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(cartDto, result.Value);
        }

        [Fact]
        public async Task UpdateItem_NullDto_ReturnsBadRequest()
        {
            var cartServiceMock = new Mock<ICartService>();
            var mapperMock = new Mock<IMapper>();
            var controller = CreateController(cartServiceMock, mapperMock);

            var result = await controller.UpdateItem("p1", null, CancellationToken.None);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task UpdateItem_Valid_CallsServiceAndReturnsNoContent()
        {
            var cartServiceMock = new Mock<ICartService>();
            cartServiceMock
                .Setup(s => s.UpdateItemQuantityAsync("c1", "p1", 3, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var mapperMock = new Mock<IMapper>();
            var controller = CreateController(cartServiceMock, mapperMock);

            var dto = new UpdateCartItemDto
            {
                CartId = "c1",
                Quantity = 3
            };

            var result = await controller.UpdateItem("p1", dto, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
            cartServiceMock.Verify(
                s => s.UpdateItemQuantityAsync("c1", "p1", 3, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task RemoveItem_MissingCartId_ReturnsBadRequest()
        {
            var cartServiceMock = new Mock<ICartService>();
            var mapperMock = new Mock<IMapper>();
            var controller = CreateController(cartServiceMock, mapperMock);

            var result = await controller.RemoveItem("p1", "", CancellationToken.None);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task RemoveItem_Valid_CallsServiceAndReturnsNoContent()
        {
            var cartServiceMock = new Mock<ICartService>();
            cartServiceMock
                .Setup(s => s.RemoveItemAsync("c1", "p1", It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var mapperMock = new Mock<IMapper>();
            var controller = CreateController(cartServiceMock, mapperMock);

            var result = await controller.RemoveItem("p1", "c1", CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
            cartServiceMock.Verify(
                s => s.RemoveItemAsync("c1", "p1", It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
