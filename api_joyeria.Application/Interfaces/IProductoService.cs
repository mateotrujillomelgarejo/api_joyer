using api_joyeria.Application.DTOs;

namespace api_joyeria.Application.Interfaces;

public interface IProductoService
{
    Task<IEnumerable<ProductoDto>> GetAllAsync(CancellationToken ct = default);
    Task<ProductoDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ProductoDto> CreateAsync(ProductoDto dto, CancellationToken ct = default);
    Task UpdateAsync(int id, ProductoDto dto, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}