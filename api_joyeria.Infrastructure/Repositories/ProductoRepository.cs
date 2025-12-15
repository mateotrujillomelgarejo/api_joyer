using api_joyeria.Application.Interfaces;
using api_joyeria.Domain.Entities;
using api_joyeria.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace api_joyeria.Infrastructure.Repositories;

public class ProductoRepository : GenericRepository<Producto>, IProductoRepository
{
    public ProductoRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Producto>> GetByNameAsync(string name, CancellationToken ct = default)
        => await _dbSet.Where(p => p.Nombre.Contains(name)).ToListAsync(ct);
}
