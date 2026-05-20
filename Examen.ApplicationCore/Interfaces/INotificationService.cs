using System.Threading.Tasks;
using Examen.ApplicationCore.Domain;

namespace Examen.ApplicationCore.Interfaces
{
    public interface INotificationService
    {
        Task EnvoyerAlerteAsync(Alerte alerte);
        Task EnvoyerEmailAsync(string destinataire, string sujet, string corps, int alerteId);
        Task EnvoyerSmsAsync(string numeroTel, string message, int alerteId);
        Task EnvoyerInAppAsync(string userId, string role, object payload);
        void MarquerCommeLu(int notificationId);
        Task MarquerCommeLuAsync(int id);
        Task RetryFailedNotificationsAsync();
    }
}