using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.Interfaces;
using Examen.Infrastructure.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Examen.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly ExamenDbContext _db;
        private readonly INotificationService _notifService;

        public NotificationsController(
            ExamenDbContext db,
            INotificationService notifService)
        {
            _db = db;
            _notifService = notifService;
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
                    .Include(h => h.Alerte)
                    .AsQueryable();

                if (canal.HasValue)
                    query = query.Where(h => h.Canal == canal.Value);

                if (statut.HasValue)
                    query = query.Where(h => h.Statut == statut.Value);

                var total = await query.CountAsync();

                var items = await query
                    .OrderByDescending(h => h.DateEnvoi ?? h.Alerte.DateAlerte)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(h => new
                    {
                        h.Id,
                        h.AlerteId,
                        Canal = h.Canal.ToString(),
                        h.Destinataire,
                        h.Sujet,
                        Statut = h.Statut.ToString(),
                        h.DateEnvoi,
                        h.DateLecture,
                        h.ErreurMessage,
                        h.NbTentatives,
                        NiveauAlerte = h.Alerte.Niveau.ToString(),
                        h.Alerte.CodeMachine
                    })
                    .ToListAsync();

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

        // GET api/notifications/non-lues/5
        [HttpGet("non-lues/{userId}")]
        public async Task<IActionResult> GetNonLues(   // ← async Task
            int userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var query = _db.HistoriqueNotifications          // ← _db, pas _unitOfWork
                    .Include(n => n.Alerte)
                    .Where(n => n.UtilisateurId == userId
                             && n.Statut != StatutNotification.Lu);

                var total = await query.CountAsync();

                var items = await query
                    .OrderByDescending(n => n.DateEnvoi)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(n => new
                    {
                        id = n.Id,
                        alerteId = n.AlerteId,
                        codeMachine = n.Alerte.CodeMachine,
                        corps = n.Corps,
                        sujet = n.Sujet,
                        statut = n.Statut.ToString(),
                        canal = n.Canal.ToString(),
                        niveau = n.Alerte.Niveau.ToString(),
                        dateAlerte = n.DateEnvoi ?? n.Alerte.DateAlerte,
                    })
                    .ToListAsync();                               // ← async

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

        // PUT api/notifications/5/lire
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

        // PUT api/notifications/lire-tout/5
        [HttpPut("lire-tout/{userId}")]
        public async Task<IActionResult> MarquerToutLu(int userId)
        {
            try
            {
                var notifs = await _db.HistoriqueNotifications
                    .Where(h =>
                        h.UtilisateurId == userId &&
                        h.Canal == CanalNotification.InApp &&
                        h.Statut != StatutNotification.Lu)
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
    }
}