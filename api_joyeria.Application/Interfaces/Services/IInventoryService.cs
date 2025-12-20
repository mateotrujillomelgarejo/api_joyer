using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace api_joyeria.Application.Interfaces.Services;

public interface IInventoryService
{
    // Check availability for items (best-effort)
    Task<bool> CheckAvailabilityAsync(IEnumerable<(int productId, int qty)> items, CancellationToken ct = default);

    // Reserve or decrement stock as part of transaction
    Task ReserveOrDecrementAsync(IEnumerable<(int productId, int qty)> items, CancellationToken ct = default);
}