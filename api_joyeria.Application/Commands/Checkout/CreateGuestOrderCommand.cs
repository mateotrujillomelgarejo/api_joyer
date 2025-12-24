using api_joyeria.Application.DTOs;
using api_joyeria.Application.DTOs.Checkout;
using MediatR;
using System.Collections.Generic;

namespace api_joyeria.Application.Commands.Checkout
{
    // Command: intención de crear una orden guest (DTO-like)
    public class CreateGuestOrderCommand : IRequest<CheckoutResponseDto>
    {
        public string CartId { get; set; }
        public string Email { get; set; }
        public AddressDto ShippingAddress { get; set; }
        public List<CreateGuestOrderItemDto> Items { get; set; } = new();
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
    }
}