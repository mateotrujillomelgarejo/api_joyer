using Microsoft.AspNetCore.Http;

namespace api_joyeria.Application.Interfaces.Services
{
    public interface IStorageService
    {
        /// <summary>
        /// Sube una imagen y devuelve la URL pública.
        /// </summary>
        Task<string> UploadImageAsync(IFormFile file, CancellationToken cancellationToken = default);

        /// <summary>
        /// (Opcional) Borra una imagen por su identificador público.
        /// </summary>
        Task DeleteImageAsync(string publicId, CancellationToken cancellationToken = default);
    }
}