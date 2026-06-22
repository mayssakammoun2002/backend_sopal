using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Examen.Infrastructure.Services
{
    public class EmailNotificationService : INotificationService
    {
        private readonly EmailSettings _settings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<EmailNotificationService> _logger;

        public EmailNotificationService(
            IOptions<EmailSettings> settings,
            IUnitOfWork unitOfWork,
            ILogger<EmailNotificationService> logger)
        {
            _settings = settings.Value;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // ── Interface INotificationService ───────────────────────────────────

        public async Task EnvoyerAlerteAsync(Alerte alerte)
        {
            // Récupérer tous les admins avec un email valide
            var adminEmails = _unitOfWork.Repository<Utilisateur>()
                .GetAll()
                .Where(u => u.Role == Role.Admin
                         && !string.IsNullOrEmpty(u.Email))
                .Select(u => u.Email!)
                .Distinct()
                .ToList();

            if (!adminEmails.Any())
            {
                _logger.LogWarning("Aucun admin avec email pour alerte #{Id}", alerte.Id);
                return;
            }

            string sujet = BuildSubject(alerte);
            string corps = BuildHtmlBody(alerte);

            foreach (var email in adminEmails)
            {
                // 1. Créer l'historique en attente
                var historique = new HistoriqueNotification
                {
                    AlerteId = alerte.Id,
                    Canal = CanalNotification.Email,
                    Destinataire = email,
                    Sujet = sujet,
                    Corps = corps,
                    Statut = StatutNotification.EnAttente,
                    NbTentatives = 0,
                    DateEnvoi = DateTime.UtcNow
                };

                _unitOfWork.Repository<HistoriqueNotification>().Add(historique);
                _unitOfWork.Save();

                // 2. Envoyer et mettre à jour le statut
                await EnvoyerEtMettreAJourAsync(historique, email, sujet, corps);
            }
        }

        public async Task MarquerCommeLuAsync(int notificationId)
        {
            var notif = _unitOfWork.Repository<HistoriqueNotification>()
                .GetAll()
                .FirstOrDefault(n => n.Id == notificationId)
                ?? throw new KeyNotFoundException($"Notification {notificationId} introuvable.");

            notif.Statut = StatutNotification.Lu;
            notif.DateLecture = DateTime.UtcNow;

            _unitOfWork.Repository<HistoriqueNotification>().Update(notif);
            _unitOfWork.Save();

            await Task.CompletedTask;
        }

        // ── Envoi SMTP interne (pas dans l'interface) ─────────────────────────

        private async Task EnvoyerEtMettreAJourAsync(
            HistoriqueNotification historique,
            string to,
            string sujet,
            string corps)
        {
            historique.NbTentatives++;

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(
                    _settings.FromDisplayName,
                    _settings.FromAddress));
                message.To.Add(MailboxAddress.Parse(to));
                message.Subject = sujet;
                message.Body = new BodyBuilder { HtmlBody = corps }.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(
                    _settings.SmtpHost,
                    _settings.SmtpPort,
                    SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_settings.UserName, _settings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                historique.Statut = StatutNotification.Envoye;
                historique.DateEnvoi = DateTime.UtcNow;
                historique.ErreurMessage = null;

                _logger.LogInformation("✅ Email envoyé à {To} pour alerte #{Id}",
                    to, historique.AlerteId);
            }
            catch (Exception ex)
            {
                historique.Statut = StatutNotification.Echec;
                historique.ErreurMessage = ex.Message;

                _logger.LogError(ex, "❌ Échec envoi email à {To}", to);
            }
            finally
            {
                // Mettre à jour le statut dans les deux cas
                _unitOfWork.Repository<HistoriqueNotification>().Update(historique);
                _unitOfWork.Save();
            }
        }

        // ── Builders ──────────────────────────────────────────────────────────

        private static string BuildSubject(Alerte alerte)
        {
            string niveau = alerte.Niveau switch
            {
                NiveauAlerte.Urgence => "URGENT",
                NiveauAlerte.Critique => "CRITIQUE",
                NiveauAlerte.Avertissement => "Avertissement",
                _ => "Info"
            };
            return $"[SOPAL Qualité] {niveau} — {alerte.CodeMachine} / {alerte.CodeArticle} — Alerte #{alerte.Id}";
        }

        private static string BuildHtmlBody(Alerte alerte)
        {
            string couleur = alerte.Niveau switch
            {
                NiveauAlerte.Urgence => "#dc2626",
                NiveauAlerte.Critique => "#d97706",
                NiveauAlerte.Avertissement => "#ca8a04",
                _ => "#6b7280"
            };

            string niveauLabel = alerte.Niveau switch
            {
                NiveauAlerte.Urgence => "URGENT",
                NiveauAlerte.Critique => "CRITIQUE",
                NiveauAlerte.Avertissement => "AVERTISSEMENT",
                _ => "INFO"
            };

            string ligneOF = !string.IsNullOrWhiteSpace(alerte.NumOF)
                ? $@"<tr>
                       <td style='padding:8px 16px;color:#6b7280;font-size:13px;border-bottom:1px solid #f3f4f6;'>Ordre de fabrication</td>
                       <td style='padding:8px 16px;font-weight:600;font-size:13px;border-bottom:1px solid #f3f4f6;'>{alerte.NumOF}</td>
                     </tr>"
                : "";

            string ligneNC = alerte.NbNonConformesConsecutifs > 0
                ? $@"<tr>
                       <td style='padding:8px 16px;color:#6b7280;font-size:13px;border-bottom:1px solid #f3f4f6;'>NC consécutifs</td>
                       <td style='padding:8px 16px;font-weight:700;font-size:13px;color:{couleur};border-bottom:1px solid #f3f4f6;'>
                           {alerte.NbNonConformesConsecutifs} contrôles Non Conformes
                       </td>
                     </tr>"
                : "";

            return $@"<!DOCTYPE html>
<html lang='fr'>
<head>
  <meta charset='UTF-8'/>
  <meta name='viewport' content='width=device-width,initial-scale=1.0'/>
</head>
<body style='margin:0;padding:0;background:#f3f4f6;font-family:Arial,Helvetica,sans-serif;'>
  <table width='100%' cellpadding='0' cellspacing='0' style='background:#f3f4f6;padding:32px 0;'>
  <tr><td align='center'>
    <table width='600' cellpadding='0' cellspacing='0'
           style='background:#fff;border-radius:8px;box-shadow:0 1px 4px rgba(0,0,0,.08);max-width:600px;width:100%;'>

      <!-- HEADER -->
      <tr>
        <td style='background:{couleur};padding:24px 32px;border-radius:8px 8px 0 0;'>
          <p style='margin:0;color:rgba(255,255,255,.85);font-size:12px;
                     text-transform:uppercase;letter-spacing:1px;'>
            Système de Contrôle Qualité
          </p>
          <h1 style='margin:6px 0 0;color:#fff;font-size:22px;font-weight:700;'>
            Alerte #{alerte.Id} — {niveauLabel}
          </h1>
        </td>
      </tr>

      <!-- BODY -->
      <tr>
        <td style='padding:28px 32px;'>
          <p style='margin:0 0 20px;font-size:15px;color:#374151;line-height:1.6;'>
            {alerte.Message}
          </p>

          <table width='100%' cellpadding='0' cellspacing='0'
                 style='border:1px solid #e5e7eb;border-radius:6px;
                         overflow:hidden;margin-bottom:24px;'>
            <tr style='background:#f9fafb;'>
              <td style='padding:10px 16px;font-size:12px;font-weight:700;color:#6b7280;
                          text-transform:uppercase;letter-spacing:.5px;
                          border-bottom:1px solid #e5e7eb;' width='45%'>Champ</td>
              <td style='padding:10px 16px;font-size:12px;font-weight:700;color:#6b7280;
                          text-transform:uppercase;letter-spacing:.5px;
                          border-bottom:1px solid #e5e7eb;'>Valeur</td>
            </tr>
            <tr>
              <td style='padding:8px 16px;color:#6b7280;font-size:13px;border-bottom:1px solid #f3f4f6;'>Machine</td>
              <td style='padding:8px 16px;font-weight:600;font-size:13px;border-bottom:1px solid #f3f4f6;'>{alerte.CodeMachine}</td>
            </tr>
            <tr style='background:#fafafa;'>
              <td style='padding:8px 16px;color:#6b7280;font-size:13px;border-bottom:1px solid #f3f4f6;'>Code Article</td>
              <td style='padding:8px 16px;font-weight:600;font-size:13px;border-bottom:1px solid #f3f4f6;'>{alerte.CodeArticle ?? "—"}</td>
            </tr>
            {ligneOF}
            <tr>
              <td style='padding:8px 16px;color:#6b7280;font-size:13px;border-bottom:1px solid #f3f4f6;'>Taux NC détecté</td>
              <td style='padding:8px 16px;font-weight:700;font-size:14px;color:{couleur};border-bottom:1px solid #f3f4f6;'>
                {alerte.TauxDetecte:F1} %
              </td>
            </tr>
            {ligneNC}
            <tr style='background:#fafafa;'>
              <td style='padding:8px 16px;color:#6b7280;font-size:13px;border-bottom:1px solid #f3f4f6;'>Défauts / Contrôles</td>
              <td style='padding:8px 16px;font-weight:600;font-size:13px;border-bottom:1px solid #f3f4f6;'>
                {alerte.QuantiteDefauts} / {alerte.QuantiteTotale}
              </td>
            </tr>
            <tr>
              <td style='padding:8px 16px;color:#6b7280;font-size:13px;'>Date alerte</td>
              <td style='padding:8px 16px;font-weight:600;font-size:13px;'>
                {alerte.DateAlerte:dd/MM/yyyy HH:mm} UTC
              </td>
            </tr>
          </table>

          <div style='text-align:center;'>
            <a href='http://localhost:5173/alertes/{alerte.Id}'
               style='display:inline-block;background:{couleur};color:#fff;
                       font-size:14px;font-weight:700;text-decoration:none;
                       padding:12px 32px;border-radius:6px;'>
              Consulter l'alerte
            </a>
          </div>
        </td>
      </tr>

      <!-- FOOTER -->
      <tr>
        <td style='background:#f9fafb;border-top:1px solid #e5e7eb;
                    padding:16px 32px;text-align:center;border-radius:0 0 8px 8px;'>
          <p style='margin:0;font-size:11px;color:#9ca3af;'>
            Message généré automatiquement par le système qualité SOPAL.<br/>
            Ne pas répondre. Contact : qualite@sopal.tn
          </p>
        </td>
      </tr>

    </table>
  </td></tr>
  </table>
</body>
</html>";
        }

        public Task EnvoyerEmailAsync(string v1, string v2, string v3, int v4)
        {
            throw new NotImplementedException();
        }
    }
}