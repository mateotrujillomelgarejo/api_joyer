using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using api_joyeria.Application.Interfaces.Repositories;
using api_joyeria.Domain.Entities;
using api_joyeria.Infrastructure.Persistence;

namespace api_joyeria.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _ctx;

        public PaymentRepository(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
        {
            await _ctx.Payments.AddAsync(payment, cancellationToken);
        }

        public async Task<Payment> GetByReferenceAsync(string reference, CancellationToken cancellationToken = default)
        {
            return await _ctx.Payments.FirstOrDefaultAsync(p => p.Reference == reference, cancellationToken);
        }

        public Task UpdateAsync(Payment payment, CancellationToken cancellationToken = default)
        {
            _ctx.Payments.Update(payment);
            return Task.CompletedTask;
        }
    }
}