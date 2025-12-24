using api_joyeria.Application.Interfaces.Repositories;
using api_joyeria.Domain.Entities;
using api_joyeria.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace api_joyeria.Infrastructure.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductoRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Producto?> GetByIdAsync(string productId, CancellationToken cancellationToken = default)
        {
            return await _db.Set<Producto>()
                            .AsNoTracking()
                            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
        }


        public async Task<IEnumerable<Producto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.Set<Producto>()
                            .AsNoTracking()
                            .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Producto>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 50;

            return await _db.Set<Producto>()
                            .AsNoTracking()
                            .OrderBy(p => p.Nombre)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync(cancellationToken);
        }

        public async Task UpdateImageUrlAsync(string id, string? imageUrl, CancellationToken cancellationToken = default)
        {
            var producto = await _db.Productos.FindAsync(new object[] { id }, cancellationToken);
            if (producto == null) return;

            producto.SetImageUrl(imageUrl);
            _db.Productos.Update(producto);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}