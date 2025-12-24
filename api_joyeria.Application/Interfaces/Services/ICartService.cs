using System.Threading;
using System.Threading.Tasks;
using api_joyeria.Application.DTOs.api_joyeria.Api.Dtos;
using api_joyeria.Domain.Entities;

namespace api_joyeria.Application.Interfaces.Services
{
    public interface ICartService
    {
        Task<string> AddItemAsync(string? cartId, string productId, int quantity, CancellationToken cancellationToken = default);
        Task<CartDto?> GetCartAsync(string cartId, CancellationToken cancellationToken = default);
        Task UpdateItemQuantityAsync(string cartId, string productId, int quantity, CancellationToken cancellationToken = default);
        Task RemoveItemAsync(string cartId, string productId, CancellationToken cancellationToken = default);
    }
}