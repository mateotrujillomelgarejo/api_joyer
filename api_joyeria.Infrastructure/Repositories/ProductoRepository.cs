using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using api_joyeria.Application.Interfaces.Repositories;
using api_joyeria.Domain.Entities;
using api_joyeria.Infrastructure.Persistence;

namespace api_joyeria.Infrastructure.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly ApplicationDbContext _ctx;

        public ProductoRepository(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IEnumerable<Producto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _ctx.Productos
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Producto> GetByIdAsync(string productId, CancellationToken cancellationToken = default)
        {
            return await _ctx.Productos.FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
        }
    }
}