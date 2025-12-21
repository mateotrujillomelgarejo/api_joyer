using MediatR;
using System.Text.Json;

namespace api_joyeria.Application.Commands.Payments
{
    public class ConfirmPaymentCommand : IRequest
    {
        public string PaymentReference { get; set; }
        public string GatewayStatus { get; set; }
        public string OrderId { get; set; }
        public JsonElement Payload { get; set; }
    }
}