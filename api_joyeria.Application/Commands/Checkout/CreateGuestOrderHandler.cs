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
    public class CreateGuestOrderHandler : IRequestHandler<CreateGuestOrderCommand, CheckoutResponseDto>
    {
        private readonly ICheckoutService _checkoutService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        public CreateGuestOrderHandler(ICheckoutService checkoutService, IUnitOfWork unitOfWork, IPaymentService paymentService)
        {
            _checkoutService = checkoutService ?? throw new ArgumentNullException(nameof(checkoutService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        }

        public async Task<CheckoutResponseDto> Handle(CreateGuestOrderCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var result = await _checkoutService.CreateGuestOrderAsync(request, cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);

                if (!string.IsNullOrWhiteSpace(request.ReturnUrl))
                {
                    try
                    {
                        var paymentInit = await _paymentService.InitializePaymentAsync(result.OrderId, request.ReturnUrl, request.CancelUrl, cancellationToken);
                        result.Payment = paymentInit;
                    }
                    catch
                    {
                        // Política: no revertimos la orden si falla la inicialización de gateway.
                        // Propagamos la excepción para que el caller la perciba (o podrías loggear y degradar devolviendo Payment = null).
                        throw;
                    }
                }

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