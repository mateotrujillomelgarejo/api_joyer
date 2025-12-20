using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace api_joyeria.Application.Interfaces;

public interface IUnitOfWork
{
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default);
    Task CommitAsync(IDbContextTransaction transaction, CancellationToken ct = default);
    Task RollbackAsync(IDbContextTransaction transaction, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}