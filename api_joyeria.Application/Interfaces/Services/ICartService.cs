using System.Threading;
using System.Threading.Tasks;
using api_joyeria.Domain.Entities;

namespace api_joyeria.Application.Interfaces.Services
{
    public interface ICartService
    {
        Task AddItemAsync(string cartId, string productId, int quantity, CancellationToken cancellationToken = default);
        Task<Cart> GetCartAsync(string cartId, CancellationToken cancellationToken = default);
        Task UpdateItemQuantityAsync(string cartId, string productId, int quantity, CancellationToken cancellationToken = default);
        Task RemoveItemAsync(string cartId, string productId, CancellationToken cancellationToken = default);
    }
}