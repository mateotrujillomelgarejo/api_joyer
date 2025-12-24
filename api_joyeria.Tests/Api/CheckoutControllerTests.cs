using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using api_joyeria.Api.Controllers;
using api_joyeria.Application.Commands.Checkout;
using api_joyeria.Application.DTOs.Checkout;
using api_joyeria.Api.Dtos;

namespace api_joyeria.Tests.Api
{
    public class CheckoutControllerTests
    {
        [Fact]
        public async Task CreateGuestOrder_NullDto_ReturnsBadRequest()
        {
            var mockMediator = new Mock<IMediator>();
            var mockMapper = new Mock<IMapper>();
            var controller = new CheckoutController(mockMediator.Object, mockMapper.Object);

            var result = await controller.CreateGuestOrder(null);
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task CreateGuestOrder_ValidDto_InvokesMediatorAndReturnsCreated()
        {
            var mockMediator = new Mock<IMediator>();
            var mockMapper = new Mock<IMapper>();

            var dto = new GuestCheckoutDto { CartId = "c1", Email = "a@b.com" };
            var command = new CreateGuestOrderCommand { CartId = "c1", Email = "a@b.com" };
            var serviceResult = new CheckoutResponseDto { OrderId = "o1", Total = 100m, Status = "Pending" };
            var apiResult = new CheckoutResultDto { OrderId = "o1", Total = 100m, Status = "Pending" };

            mockMapper.Setup(m => m.Map<CreateGuestOrderCommand>(It.IsAny<GuestCheckoutDto>())).Returns(command);
            mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(serviceResult);
            mockMapper.Setup(m => m.Map<CheckoutResultDto>(serviceResult)).Returns(apiResult);

            var controller = new CheckoutController(mockMediator.Object, mockMapper.Object);

            var result = await controller.CreateGuestOrder(dto) as CreatedAtActionResult;
            Assert.NotNull(result);
            Assert.Equal("GetOrderStatus", result.ActionName);
            Assert.Equal(apiResult, result.Value);

            mockMediator.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}