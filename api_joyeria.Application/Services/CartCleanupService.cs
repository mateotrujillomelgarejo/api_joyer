using api_joyeria.Application.Interfaces.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace api_joyeria.Application.Services;

public class CartCleanupService : BackgroundService
{
    private readonly ICartRepository _cartRepo;
    private readonly ILogger<CartCleanupService> _logger;
    private readonly TimeSpan _period = TimeSpan.FromMinutes(30);

    public CartCleanupService(ICartRepository cartRepo, ILogger<CartCleanupService> logger)
    {
        _cartRepo = cartRepo;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CartCleanupService started");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _cartRepo.DeleteExpiredAsync(DateTime.UtcNow, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning expired carts");
            }
            await Task.Delay(_period, stoppingToken);
        }
    }
}