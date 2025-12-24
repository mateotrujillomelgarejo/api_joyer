using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using api_joyeria.Api.Controllers;
using api_joyeria.Application.Commands.Payments;
using api_joyeria.Application.Interfaces;

namespace api_joyeria.Tests.Api
{
    public class WebhookControllerTests
    {
        [Fact]
        public async Task ReceiveIzipay_InvalidSignature_ReturnsBadRequestAndDoesNotSendCommand()
        {
            var mockMediator = new Mock<IMediator>();
            var mockGateway = new Mock<IPaymentGateway>();
            var mockLogger = new Mock<ILogger<WebhookController>>();

            mockGateway.Setup(g => g.ValidateNotificationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var controller = new WebhookController(mockMediator.Object, mockGateway.Object, mockLogger.Object);

            var payload = JsonDocument.Parse("{\"payment_id\":\"p\",\"order_id\":\"o\",\"status\":\"SUCCESS\"}").RootElement;
            var result = await controller.ReceiveIzipay("bad", payload);

            Assert.IsType<BadRequestResult>(result);
            mockMediator.Verify(m => m.Send(It.IsAny<ConfirmPaymentCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ReceiveIzipay_MissingFields_ReturnsBadRequest()
        {
            var mockMediator = new Mock<IMediator>();
            var mockGateway = new Mock<IPaymentGateway>();
            var mockLogger = new Mock<ILogger<WebhookController>>();

            mockGateway.Setup(g => g.ValidateNotificationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var controller = new WebhookController(mockMediator.Object, mockGateway.Object, mockLogger.Object);

            var payload = JsonDocument.Parse("{\"foo\":\"bar\"}").RootElement;
            var result = await controller.ReceiveIzipay("sig", payload);

            Assert.IsType<BadRequestResult>(result);
            mockMediator.Verify(m => m.Send(It.IsAny<ConfirmPaymentCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ReceiveIzipay_ValidPayload_ReturnsOk_AndDispatchesCommandInBackground()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator
            .Setup(m => m.Send(It.IsAny<ConfirmPaymentCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);


            var mockGateway = new Mock<IPaymentGateway>();
            mockGateway.Setup(g => g.ValidateNotificationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var mockLogger = new Mock<ILogger<WebhookController>>();
            var controller = new WebhookController(mockMediator.Object, mockGateway.Object, mockLogger.Object);

            var payload = JsonDocument.Parse("{\"payment_id\":\"p123\",\"order_id\":\"o123\",\"status\":\"SUCCESS\"}").RootElement;
            var result = await controller.ReceiveIzipay("good", payload);

            Assert.IsType<OkResult>(result);

            // Wait a short time for background Task.Run to execute and call mediator.Send
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var verified = false;
            while (sw.ElapsedMilliseconds < 500)
            {
                try
                {
                    mockMediator.Verify(m => m.Send(It.IsAny<ConfirmPaymentCommand>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
                    verified = true;
                    break;
                }
                catch (MockException)
                {
                    await Task.Delay(50);
                }
            }

            Assert.True(verified, "Mediator.Send was not invoked by the background webhook handler.");
        }
    }
}