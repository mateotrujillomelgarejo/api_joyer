using AutoMapper;
using api_joyeria.Application.DTOs;
using api_joyeria.Application.Interfaces;
using api_joyeria.Domain.Entities;

namespace api_joyeria.Application.Services;

public class ProductoService : IProductoService
{
    private readonly IProductoRepository _repo;
    private readonly IMapper _mapper;

    public ProductoService(IProductoRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<ProductoDto> CreateAsync(ProductoDto dto, CancellationToken ct = default)
    {
        var entity = _mapper.Map<Producto>(dto);
        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);
        return _mapper.Map<ProductoDto>(entity);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        if (entity == null) throw new KeyNotFoundException($"Producto {id} no encontrado.");
        _repo.Remove(entity);
        await _repo.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<ProductoDto>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await _repo.GetAllAsync(ct);
        return _mapper.Map<IEnumerable<ProductoDto>>(list);
    }

    public async Task<ProductoDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        return entity is null ? null : _mapper.Map<ProductoDto>(entity);
    }

    public async Task UpdateAsync(int id, ProductoDto dto, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        if (entity == null) throw new KeyNotFoundException($"Producto {id} no encontrado.");
        _mapper.Map(dto, entity);
        _repo.Update(entity);
        await _repo.SaveChangesAsync(ct);
    }
}