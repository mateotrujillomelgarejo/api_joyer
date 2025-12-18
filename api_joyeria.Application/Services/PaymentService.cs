using api_joyeria.Application.DTOs;
using api_joyeria.Application.Interfaces;
using api_joyeria.Domain.Entities;
using AutoMapper;

namespace api_joyeria.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IRepository<Payment> _paymentRepository;
    private readonly IMapper _mapper;

    public PaymentService(IRepository<Payment> paymentRepository, IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<PaymentDto> CreatePaymentAsync(PaymentDto dto, CancellationToken ct = default)
    {
        var payment = _mapper.Map<Payment>(dto);

        await _paymentRepository.AddAsync(payment, ct);
        await _paymentRepository.SaveChangesAsync(ct);

        return _mapper.Map<PaymentDto>(payment);
    }

    public async Task<PaymentDto?> GetPaymentByIdAsync(int id, CancellationToken ct = default)
    {
        var payment = await _paymentRepository.GetByIdAsync(id, ct);
        return payment is null ? null : _mapper.Map<PaymentDto>(payment);
    }
}