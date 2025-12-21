using MediatR;
using api_joyeria.Application.DTOs.Payment;

namespace api_joyeria.Application.Commands.Payments
{
    public class InitializePaymentCommand : IRequest<PaymentInitResponseDto>
    {
        public string OrderId { get; set; }
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
    }
}