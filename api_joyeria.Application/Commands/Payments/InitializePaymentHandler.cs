using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using api_joyeria.Application.Commands.Payments;
using api_joyeria.Application.Interfaces.Services;
using api_joyeria.Application.Interfaces;
using api_joyeria.Application.DTOs.Payment;

namespace api_joyeria.Application.Commands.Payments
{
    public class InitializePaymentHandler : IRequestHandler<InitializePaymentCommand, PaymentInitResponseDto>
    {
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;

        public InitializePaymentHandler(IPaymentService paymentService, IUnitOfWork unitOfWork)
        {
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<DTOs.Payment.PaymentInitResponseDto> Handle(InitializePaymentCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var res = await _paymentService.InitializePaymentAsync(request.OrderId, request.ReturnUrl, request.CancelUrl, cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);
                return res;
            }
            catch
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}