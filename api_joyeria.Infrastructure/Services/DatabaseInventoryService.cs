using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using api_joyeria.Application.Interfaces.Services;
using api_joyeria.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace api_joyeria.Infrastructure.Services;

public class DatabaseInventoryService : IInventoryService
{
    private readonly ApplicationDbContext _ctx;
    public DatabaseInventoryService(ApplicationDbContext ctx) => _ctx = ctx;

    public async Task<bool> CheckAvailabilityAsync(IEnumerable<(int productId, int qty)> items, CancellationToken ct = default)
    {
        var ids = items.Select(i => i.productId).Distinct().ToList();
        var products = await _ctx.Productos.Where(p => ids.Contains(p.Id)).ToListAsync(ct);
        foreach (var it in items)
        {
            var p = products.FirstOrDefault(x => x.Id == it.productId);
            if (p == null || p.Stock < it.qty) return false;
        }
        return true;
    }

    public async Task ReserveOrDecrementAsync(IEnumerable<(int productId, int qty)> items, CancellationToken ct = default)
    {
        // Simple approach: decrement stock in DB. Requires being called inside a transaction for atomicity.
        var ids = items.Select(i => i.productId).Distinct().ToList();
        var products = await _ctx.Productos.Where(p => ids.Contains(p.Id)).ToListAsync(ct);

        foreach (var it in items)
        {
            var p = products.FirstOrDefault(x => x.Id == it.productId)
                ?? throw new KeyNotFoundException($"Product {it.productId} not found");
            if (p.Stock < it.qty) throw new InvalidOperationException($"Insufficient stock for product {it.productId}");
            p.Stock -= it.qty;
            _ctx.Productos.Update(p);
        }
        // Note: SaveChanges should be handled by UnitOfWork/transaction caller
    }
}