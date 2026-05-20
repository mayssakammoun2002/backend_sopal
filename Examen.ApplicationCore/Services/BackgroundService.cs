using Examen.ApplicationCore.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Examen.Infrastructure
{
    /// <summary>
    /// Service en arrière-plan qui vérifie les seuils d'alertes périodiquement
    /// </summary>
    public class AlerteBackgroundService : BackgroundService
    {
        private readonly ILogger<AlerteBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5); // Vérifier toutes les 5 minutes

        public AlerteBackgroundService(ILogger<AlerteBackgroundService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🚀 AlerteBackgroundService démarré");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var alerteService = scope.ServiceProvider.GetRequiredService<AlerteService>();

                        _logger.LogInformation("⏰ Vérification des seuils d'alertes...");
                        await alerteService.VerifierSeuilsAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Erreur lors de la vérification des seuils : {ex.Message}");
                }

                // Attendre l'intervalle avant la prochaine vérification
                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("🛑 AlerteBackgroundService arrêté");
        }
    }
}