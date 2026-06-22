using System.Threading.Tasks;
using Examen.ApplicationCore.Domain;

namespace Examen.ApplicationCore.Interfaces
{
    public interface INotificationService
    {

        Task EnvoyerAlerteAsync(Alerte alerte);
        Task EnvoyerEmailAsync(string v1, string v2, string v3, int v4);
        Task MarquerCommeLuAsync(int notificationId);
    }
}