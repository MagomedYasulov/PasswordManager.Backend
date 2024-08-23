using Newtonsoft.Json.Linq;
using PasswordManager.Backend.Data;
using PasswordManager.Backend.Data.Entities;

namespace PasswordManager.Backend.Services
{
    public class TokenHostedService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TokenHostedService> _logger;
        private Timer? _timer = null;

        public TokenHostedService(ILogger<TokenHostedService> logger, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Token Hosted Service running.");
            _timer = new Timer(ClearExpiredTokens, null, TimeSpan.Zero, TimeSpan.FromHours(3));
            return Task.CompletedTask;
        }

        private void ClearExpiredTokens(object? state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IRepository>();
                repository.Delete<Token>(rt => rt.ExpiryTime <= DateTime.UtcNow);
                repository.Save();
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
