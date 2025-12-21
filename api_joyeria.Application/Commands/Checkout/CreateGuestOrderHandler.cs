using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using api_joyeria.Application.Commands.Checkout;
using api_joyeria.Application.Interfaces.Services;
using api_joyeria.Application.Interfaces;
using api_joyeria.Application.DTOs.Checkout;

namespace api_joyeria.Application.Commands.Checkout
{
    public class CreateGuestOrderHandler : IRequestHandler<CreateGuestOrderCommand,CheckoutResponseDto>
    {
        private readonly ICheckoutService _checkoutService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateGuestOrderHandler(ICheckoutService checkoutService, IUnitOfWork unitOfWork)
        {
            _checkoutService = checkoutService ?? throw new ArgumentNullException(nameof(checkoutService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<CheckoutResponseDto> Handle(CreateGuestOrderCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var result = await _checkoutService.CreateGuestOrderAsync(request, cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}