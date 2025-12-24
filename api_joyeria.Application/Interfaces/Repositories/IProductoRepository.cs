using api_joyeria.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace api_joyeria.Application.Interfaces.Repositories
{
    public interface IProductoRepository
    {
        Task<Producto?> GetByIdAsync(string productId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Producto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task UpdateImageUrlAsync(string id, string? imageUrl, CancellationToken cancellationToken = default);
        Task<IEnumerable<Producto>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    }
}