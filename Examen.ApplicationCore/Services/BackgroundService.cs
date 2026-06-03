using Examen.ApplicationCore.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Examen.Infrastructure
{
    public class AlerteBackgroundService : BackgroundService
    {
        private readonly ILogger<AlerteBackgroundService> _logger;
        private readonly IServiceProvider _sp;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

        public AlerteBackgroundService(ILogger<AlerteBackgroundService> logger, IServiceProvider sp)
        {
            _logger = logger;
            _sp = sp;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            _logger.LogInformation("🚀 AlerteBackgroundService démarré (intervalle : {I})", _interval);
            await RunCycle();
            while (!ct.IsCancellationRequested)
            {
                try { await Task.Delay(_interval, ct); }
                catch (OperationCanceledException) { break; }
                await RunCycle();
            }
            _logger.LogInformation("🛑 AlerteBackgroundService arrêté.");
        }

        private async Task RunCycle()
        {
            try
            {
                using var scope = _sp.CreateScope();
                var svc = scope.ServiceProvider.GetRequiredService<IServiceAlerte>(); // ✅ interface
                _logger.LogInformation("⏰ [{T}] Vérification des seuils...", DateTime.UtcNow);
                await svc.VerifierSeuilsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erreur cycle vérification seuils");
            }
        }
    }
}