using System.Threading;
using System.Threading.Tasks;
using api_joyeria.Application.Commands.Checkout;
using api_joyeria.Application.DTOs.Checkout;

namespace api_joyeria.Application.Interfaces.Services
{
    public interface ICheckoutService
    {
        Task<CheckoutResponseDto> CreateGuestOrderAsync(CreateGuestOrderCommand command, CancellationToken cancellationToken = default);
    }
}