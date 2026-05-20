using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.Interfaces;

public class ServiceNotification : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;

    public ServiceNotification(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task EnvoyerAlerteAsync(Alerte alerte)
    {
        var destinataires = _unitOfWork
            .Repository<DestinataireNotification>()
            .GetAll()
            .Where(d => d.EstActif)
            .ToList();

        foreach (var d in destinataires)
        {
            var notif = new HistoriqueNotification
            {
                AlerteId = alerte.Id,
                UtilisateurId = (int)d.UtilisateurId,
                Canal = d.Canal,
                Destinataire = d.Destinataire,

                Sujet = $"Alerte {alerte.CodeMachine}",

                Corps = $"Machine {alerte.CodeMachine} dépasse seuil avec {alerte.TauxDetecte}%",

                Statut = StatutNotification.Envoye,
                DateEnvoi = DateTime.UtcNow
            };

            _unitOfWork.Repository<HistoriqueNotification>().Add(notif);
        }

        _unitOfWork.Save();

        await Task.CompletedTask;
    }

    public Task EnvoyerEmailAsync(string destinataire, string sujet, string corps, int alerteId)
    {
        throw new NotImplementedException();
    }

    public Task EnvoyerInAppAsync(string userId, string role, object payload)
    {
        throw new NotImplementedException();
    }

    public Task EnvoyerSmsAsync(string numeroTel, string message, int alerteId)
    {
        throw new NotImplementedException();
    }

    public void MarquerCommeLu(int notificationId)
    {
        throw new NotImplementedException();
    }

    public Task MarquerCommeLuAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task RetryFailedNotificationsAsync()
    {
        throw new NotImplementedException();
    }
}