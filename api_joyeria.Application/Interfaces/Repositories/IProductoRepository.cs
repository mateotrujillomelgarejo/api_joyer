using api_joyeria.Domain.Entities;

namespace api_joyeria.Application.Interfaces.Repositories;

public interface IProductoRepository : IRepository<Producto>
{
    Task<IEnumerable<Producto>> GetByNameAsync(string name, CancellationToken ct = default);
}
