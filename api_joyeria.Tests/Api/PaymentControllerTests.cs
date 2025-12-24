using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using api_joyeria.Api.Controllers;
using api_joyeria.Application.Commands.Payments;
using api_joyeria.Application.DTOs.Payment;
using api_joyeria.Api.Dtos;

namespace api_joyeria.Tests.Api
{
    public class PaymentControllerTests
    {
        [Fact]
        public async Task InitializePayment_NullDto_ReturnsBadRequest()
        {
            var mockMediator = new Mock<IMediator>();
            var mockMapper = new Mock<IMapper>();
            var controller = new PaymentController(mockMediator.Object, mockMapper.Object);

            var result = await controller.InitializePayment(null);
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task InitializePayment_ValidDto_ReturnsOkAndCallsMediator()
        {
            var mockMediator = new Mock<IMediator>();
            var mockMapper = new Mock<IMapper>();

            var dto = new PaymentRequestDto { OrderId = "o1", ReturnUrl = "r", CancelUrl = "c" };
            var command = new InitializePaymentCommand { OrderId = "o1", ReturnUrl = "r", CancelUrl = "c" };
            var serviceResult = new PaymentInitResponseDto { PaymentId = "p1", PaymentUrl = "u" };
            var apiResult = new PaymentInitResultDto { PaymentId = "p1", PaymentUrl = "u" };

            mockMapper.Setup(m => m.Map<InitializePaymentCommand>(It.IsAny<PaymentRequestDto>())).Returns(command);
            mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(serviceResult);
            mockMapper.Setup(m => m.Map<PaymentInitResultDto>(serviceResult)).Returns(apiResult);

            var controller = new PaymentController(mockMediator.Object, mockMapper.Object);
            var result = await controller.InitializePayment(dto) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(apiResult, result.Value);
            mockMediator.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}