using api_joyeria.Application.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace api_joyeria.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _ctx;
    public UnitOfWork(ApplicationDbContext ctx) => _ctx = ctx;

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default)
        => await _ctx.Database.BeginTransactionAsync(ct);

    public async Task CommitAsync(IDbContextTransaction transaction, CancellationToken ct = default)
    {
        await transaction.CommitAsync(ct);
    }

    public async Task RollbackAsync(IDbContextTransaction transaction, CancellationToken ct = default)
    {
        await transaction.RollbackAsync(ct);
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        _ctx.SaveChangesAsync(ct);
}