using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.Interfaces;
using Examen.Infrastructure.Data;

namespace Examen.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly ExamenDbContext _db;
        private readonly INotificationService _notifService;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(
            ExamenDbContext db,
            INotificationService notifService,
            ILogger<NotificationsController> logger)
        {
            _db = db;
            _notifService = notifService;
            _logger = logger;
        }

        // GET api/notifications/historique
        [HttpGet("historique")]
        public async Task<IActionResult> GetHistorique(
            [FromQuery] CanalNotification? canal,
            [FromQuery] StatutNotification? statut,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 30)
        {
            try
            {
                var query = _db.HistoriqueNotifications
                    .AsNoTracking()
                    .AsQueryable();

                if (canal.HasValue)
                    query = query.Where(h => h.Canal == canal.Value);

                if (statut.HasValue)
                    query = query.Where(h => h.Statut == statut.Value);

                var total = await query.CountAsync();

                var ids = await query
                    .OrderByDescending(h => h.DateEnvoi)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(h => h.Id)
                    .ToListAsync();

                var items = await (
                    from h in _db.HistoriqueNotifications
                    where ids.Contains(h.Id)
                    join a in _db.Alertes on h.AlerteId equals a.Id into alerteJoin
                    from alerte in alerteJoin.DefaultIfEmpty()
                    orderby h.DateEnvoi descending
                    select new
                    {
                        id = h.Id,
                        alerteId = h.AlerteId,
                        canal = h.Canal.ToString(),
                        destinataire = h.Destinataire ?? "",
                        sujet = h.Sujet ?? "",
                        corps = h.Corps ?? "",
                        statut = h.Statut.ToString(),
                        dateEnvoi = h.DateEnvoi,
                        dateLecture = h.DateLecture,
                        erreurMessage = h.ErreurMessage ?? "",
                        nbTentatives = h.NbTentatives,
                        niveauAlerte = alerte != null ? alerte.Niveau.ToString() : "—",
                        codeMachine = alerte != null ? alerte.CodeMachine : "—"
                    }
                ).ToListAsync();

                return Ok(new { total, page, pageSize, items });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }

        // GET api/notifications/non-lues/{userId}
        [HttpGet("non-lues/{userId}")]
        public async Task<IActionResult> GetNonLues(
            int userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var query = _db.HistoriqueNotifications
                    .Include(n => n.Alerte)
                    .Where(n => n.UtilisateurId == userId
                             && n.Statut != StatutNotification.Lu);

                var total = await query.CountAsync();

                var rawItems = await query
                    .OrderByDescending(n => n.DateEnvoi)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var items = rawItems.Select(n => new
                {
                    id = n.Id,
                    alerteId = n.AlerteId,
                    codeMachine = n.Alerte != null ? n.Alerte.CodeMachine : "—",
                    corps = n.Corps ?? "",
                    sujet = n.Sujet ?? "",
                    statut = n.Statut.ToString(),
                    canal = n.Canal.ToString(),
                    niveau = n.Alerte != null ? n.Alerte.Niveau.ToString() : "—",
                    dateAlerte = n.DateEnvoi ?? (n.Alerte != null
                                    ? n.Alerte.DateAlerte
                                    : DateTime.UtcNow)
                }).ToList();

                return Ok(new { items, total, page, pageSize });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }

        // PUT api/notifications/{id}/lire
        [HttpPut("{id}/lire")]
        public async Task<IActionResult> MarquerLu(int id)
        {
            try
            {
                await _notifService.MarquerCommeLuAsync(id);
                return Ok(new { success = true, message = "Notification marquée comme lue" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }

        // PUT api/notifications/lire-tout/{userId}
        [HttpPut("lire-tout/{userId}")]
        public async Task<IActionResult> MarquerToutLu(int userId)
        {
            try
            {
                var notifs = await _db.HistoriqueNotifications
                    .Where(h => h.UtilisateurId == userId
                             && h.Canal == CanalNotification.InApp
                             && h.Statut != StatutNotification.Lu)
                    .ToListAsync();

                foreach (var n in notifs)
                {
                    n.Statut = StatutNotification.Lu;
                    n.DateLecture = DateTime.UtcNow;
                }

                await _db.SaveChangesAsync();
                return Ok(new { success = true, message = "Toutes les notifications ont été marquées comme lues" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }
        // POST api/notifications/test-email
        // POST api/notifications/test-email
        [HttpPost("test-email")]
        public async Task<IActionResult> TestEmail()
        {
            try
            {
                // Prendre la dernière alerte réelle en base
                var alerte = await _db.Alertes
                    .OrderByDescending(a => a.DateAlerte)
                    .FirstOrDefaultAsync();

                if (alerte == null)
                    return BadRequest(new
                    {
                        success = false,
                        message = "Aucune alerte en base. Crée d'abord une alerte via POST /api/alertes."
                    });

                await _notifService.EnvoyerAlerteAsync(alerte);

                return Ok(new
                {
                    success = true,
                    message = $"Email de test envoyé pour alerte #{alerte.Id} ({alerte.CodeMachine})"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    erreur = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }
        // POST api/notifications/envoyer-alerte/{alerteId}
        [HttpPost("envoyer-alerte/{alerteId}")]
        public async Task<IActionResult> EnvoyerAlerte(int alerteId)
        {
            try
            {
                var alerte = await _db.Alertes.FindAsync(alerteId);
                if (alerte == null)
                    return NotFound(new { message = $"Alerte #{alerteId} introuvable." });

                await _notifService.EnvoyerAlerteAsync(alerte);

                return Ok(new { success = true, message = $"Email envoyé pour alerte #{alerteId}." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur envoi alerte #{AlerteId}", alerteId);
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }
    }
}