using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Examen.Infrastructure.Services
{
    public class ServiceNotification : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly SmtpSettings _smtp;
        private readonly ILogger<ServiceNotification> _logger;

        public ServiceNotification(
            IUnitOfWork unitOfWork,
            IOptions<SmtpSettings> smtpOptions,
            ILogger<ServiceNotification> logger)
        {
            _unitOfWork = unitOfWork;
            _smtp = smtpOptions.Value;
            _logger = logger;
        }

        public async Task EnvoyerAlerteAsync(Alerte alerte)
        {
            var destinataires = _unitOfWork
                .Repository<DestinataireNotification>()
                .GetAll()
                .Where(d => d.EstActif && d.NiveauMinimum <= alerte.Niveau)
                .ToList();

            foreach (var d in destinataires)
            {
                var statut = StatutNotification.EnAttente;
                string? erreur = null;

                if (d.Canal == CanalNotification.Email)
                {
                    try
                    {
                        var sujet = $"⚠️ Alerte machine {alerte.CodeMachine}";
                        var corps = $@"
                            <h2>Alerte détectée sur {alerte.CodeMachine}</h2>
                            <p>Taux de défauts détecté : <strong>{alerte.TauxDetecte}%</strong></p>
                            <p>Niveau : <strong>{alerte.Niveau}</strong></p>
                            <p>Date : {DateTime.UtcNow:dd/MM/yyyy HH:mm} UTC</p>
                            <hr/>
                            <p style='color:gray;font-size:12px'>Système SOPAL – Notification automatique</p>";

                        await EnvoyerEmailAsync(d.Destinataire, sujet, corps, alerte.Id);
                        statut = StatutNotification.Envoye;
                    }
                    catch (Exception ex)
                    {
                        statut = StatutNotification.Echec;
                        erreur = ex.Message;
                        _logger.LogError(ex, "❌ Échec envoi email à {To}", d.Destinataire);
                    }
                }
                else
                {
                    statut = StatutNotification.Envoye;
                }

                var notif = new HistoriqueNotification
                {
                    AlerteId = alerte.Id,
                    UtilisateurId = d.UtilisateurId ?? 0,
                    Canal = d.Canal,
                    Destinataire = d.Destinataire,
                    Sujet = $"Alerte {alerte.CodeMachine}",
                    Corps = $"Machine {alerte.CodeMachine} – taux détecté : {alerte.TauxDetecte}%",
                    Statut = statut,
                    ErreurMessage = erreur,
                    DateEnvoi = DateTime.UtcNow,
                    NbTentatives = 1
                };

                _unitOfWork.Repository<HistoriqueNotification>().Add(notif);
            }

            _unitOfWork.Save();
        }

        public async Task EnvoyerEmailAsync(
            string destinataire, string sujet, string corps, int alerteId)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_smtp.FromDisplayName, _smtp.FromAddress));
                message.To.Add(MailboxAddress.Parse(destinataire));
                message.Subject = sujet;
                message.Body = new BodyBuilder { HtmlBody = corps }.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_smtp.SmtpHost, _smtp.SmtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_smtp.UserName, _smtp.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("✅ Email envoyé à {To}", destinataire);

                // Sauvegarder historique seulement si alerte réelle
                if (alerteId > 0)
                {
                    var historique = new HistoriqueNotification
                    {
                        AlerteId = alerteId,
                        Canal = CanalNotification.Email,
                        Destinataire = destinataire,
                        Sujet = sujet,
                        Corps = corps,
                        Statut = StatutNotification.Envoye,
                        NbTentatives = 1,
                        DateEnvoi = DateTime.UtcNow
                    };
                    _unitOfWork.Repository<HistoriqueNotification>().Add(historique);
                    _unitOfWork.Save();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Échec envoi email à {To}", destinataire);
                throw;
            }
        }

        public async Task MarquerCommeLuAsync(int notificationId)
        {
            var notif = await Task.FromResult(
                _unitOfWork.Repository<HistoriqueNotification>()
                           .GetAll()
                           .FirstOrDefault(n => n.Id == notificationId)
            );

            if (notif == null)
                throw new KeyNotFoundException($"Notification {notificationId} introuvable.");

            notif.Statut = StatutNotification.Lu;
            notif.DateLecture = DateTime.UtcNow;

            _unitOfWork.Repository<HistoriqueNotification>().Update(notif);
            _unitOfWork.Save();
        }

        public void MarquerCommeLu(int notificationId) =>
            MarquerCommeLuAsync(notificationId).GetAwaiter().GetResult();

        public Task EnvoyerInAppAsync(string userId, string role, object payload) =>
            Task.CompletedTask;

        public Task EnvoyerSmsAsync(string numeroTel, string message, int alerteId) =>
            Task.CompletedTask;

        public Task RetryFailedNotificationsAsync() =>
            Task.CompletedTask;
    }
}