using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using api_joyeria.Application.Commands.Payments;
using api_joyeria.Application.Interfaces.Services;
using api_joyeria.Application.Interfaces;

namespace api_joyeria.Application.Commands.Payments
{
    public class ConfirmPaymentHandler : IRequestHandler<ConfirmPaymentCommand>
    {
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;

        public ConfirmPaymentHandler(IPaymentService paymentService, IUnitOfWork unitOfWork)
        {
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Unit> Handle(ConfirmPaymentCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                await _paymentService.ConfirmPaymentAsync(request.PaymentReference, request.GatewayStatus, request.OrderId, request.Payload, cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);
                return Unit.Value;
            }
            catch
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }

        Task IRequestHandler<ConfirmPaymentCommand>.Handle(ConfirmPaymentCommand request, CancellationToken cancellationToken)
        {
            return Handle(request, cancellationToken);
        }
    }
}