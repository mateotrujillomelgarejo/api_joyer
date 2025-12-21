using System.Threading;
using System.Threading.Tasks;
using api_joyeria.Domain.Entities;

namespace api_joyeria.Application.Interfaces.Repositories
{
    public interface IPaymentRepository
    {
        Task AddAsync(Payment payment, CancellationToken cancellationToken = default);
        Task<Payment> GetByReferenceAsync(string reference, CancellationToken cancellationToken = default);
        Task UpdateAsync(Payment payment, CancellationToken cancellationToken = default);
    }
}