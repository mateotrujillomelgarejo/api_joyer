using api_joyeria.Domain.Entities;

namespace api_joyeria.Application.Interfaces;

public interface IProductoRepository : IRepository<Producto>
{
    Task<IEnumerable<Producto>> GetByNameAsync(string name, CancellationToken ct = default);
}
