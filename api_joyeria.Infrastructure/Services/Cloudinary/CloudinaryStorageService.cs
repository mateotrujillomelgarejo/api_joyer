using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using api_joyeria.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace api_joyeria.Infrastructure.Services.Cloudinary
{
    public class CloudinaryStorageService : IStorageService
    {
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;

        public CloudinaryStorageService(IOptions<CloudinaryOptions> options)
        {
            var o = options.Value;

            var account = new Account(
                o.CloudName,
                o.ApiKey,
                o.ApiSecret
            );

            _cloudinary = new CloudinaryDotNet.Cloudinary(account)
            {
                Api = { Secure = true }
            };
        }

        public async Task<string> UploadImageAsync(
            IFormFile file,
            CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Archivo inválido");

            var allowed = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowed.Contains(file.ContentType))
                throw new ArgumentException("Tipo de archivo no permitido");

            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms, cancellationToken);
            ms.Position = 0;

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, ms),
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false,
                Transformation = new Transformation()
                    .Quality("auto")
                    .FetchFormat("auto")
            };

            var result = await _cloudinary.UploadAsync(uploadParams, cancellationToken);

            if (result.StatusCode != System.Net.HttpStatusCode.OK &&
                result.StatusCode != System.Net.HttpStatusCode.Created)
            {
                throw new InvalidOperationException(
                    "Error subiendo la imagen a Cloudinary: " + result.Error?.Message);
            }

            var url = result.SecureUrl?.ToString() ?? result.Url?.ToString();

            if (string.IsNullOrWhiteSpace(url))
            {
                throw new InvalidOperationException(
                    "Cloudinary no devolvió una URL válida para la imagen subida.");
            }

            return url;

        }

        public Task DeleteImageAsync(
            string publicId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                return Task.CompletedTask;

            _cloudinary.Destroy(new DeletionParams(publicId));
            return Task.CompletedTask;
        }
    }
}
