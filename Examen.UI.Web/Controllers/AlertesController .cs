using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.Interfaces;
using Examen.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Examen.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AlertesController : ControllerBase
    {
        private readonly IServiceAlerte _alerteService;
        private readonly ExamenDbContext _db;

        public AlertesController(IServiceAlerte alerteService, ExamenDbContext db)
        {
            _alerteService = alerteService;
            _db = db;
        }

        // ── GET api/alertes ──────────────────────────────────────────
        /// <summary>
        /// Liste paginée de toutes les alertes avec filtres optionnels.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] StatutAlerte? statut,
            [FromQuery] NiveauAlerte? niveau,
            [FromQuery] string? codeMachine,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var query = _db.Alertes
                    .Include(a => a.Seuil)
                    .AsQueryable();

                if (statut.HasValue)
                    query = query.Where(a => a.Statut == statut.Value);

                if (niveau.HasValue)
                    query = query.Where(a => a.Niveau == niveau.Value);

                if (!string.IsNullOrWhiteSpace(codeMachine))
                    query = query.Where(a => a.CodeMachine == codeMachine);

                var total = await query.CountAsync();

                var items = await query
                    .OrderByDescending(a => a.Niveau)
                    .ThenByDescending(a => a.DateAlerte)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(a => new AlerteDto(a))
                    .ToListAsync();

                return Ok(new { total, page, pageSize, items });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ── GET api/alertes/actives ──────────────────────────────────
        [HttpGet("actives")]
        public IActionResult GetActives()
        {
            var alertes = _alerteService.GetActives()
                .Select(a => new AlerteDto(a));
            return Ok(alertes);
        }

        // ── GET api/alertes/stats ────────────────────────────────────
        /// <summary>
        /// Retourne les compteurs résumés pour le dashboard.
        /// </summary>
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var stats = new
                {
                    TotalActives = await _db.Alertes
                        .CountAsync(a => a.Statut == StatutAlerte.Nouvelle
                                      || a.Statut == StatutAlerte.EnCours),
                    NbUrgence = await _db.Alertes
                        .CountAsync(a => a.Niveau == NiveauAlerte.Urgence
                                      && (a.Statut == StatutAlerte.Nouvelle
                                          || a.Statut == StatutAlerte.EnCours)),
                    NbCritique = await _db.Alertes
                        .CountAsync(a => a.Niveau == NiveauAlerte.Critique
                                      && (a.Statut == StatutAlerte.Nouvelle
                                          || a.Statut == StatutAlerte.EnCours)),
                    NbAvertissement = await _db.Alertes
                        .CountAsync(a => a.Niveau == NiveauAlerte.Avertissement
                                      && (a.Statut == StatutAlerte.Nouvelle
                                          || a.Statut == StatutAlerte.EnCours)),
                    NbResoluesAujourdHui = await _db.Alertes
                        .CountAsync(a => a.Statut == StatutAlerte.Resolue
                                      && a.DateResolution.HasValue
                                      && a.DateResolution.Value.Date == DateTime.UtcNow.Date),
                    MachinesEnAlerte = await _db.Alertes
                        .Where(a => a.Statut == StatutAlerte.Nouvelle
                                 || a.Statut == StatutAlerte.EnCours)
                        .Select(a => a.CodeMachine)
                        .Distinct()
                        .CountAsync()
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ── GET api/alertes/{id} ─────────────────────────────────────
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var alerte = await _db.Alertes
                    .Include(a => a.Seuil)
                    .Include(a => a.Commentaires)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (alerte is null) return NotFound(new { message = $"Alerte {id} introuvable." });

                return Ok(new AlerteDetailDto(alerte));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ── PUT api/alertes/{id}/prendre-en-charge ───────────────────
        [HttpPut("{id}/prendre-en-charge")]
        public IActionResult PrendreEnCharge(int id)
        {
            try
            {
                _alerteService.PrendreEnChargeAlerte(id, GetUserId());
                return Ok(new { success = true, message = "Alerte prise en charge." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        }

        // ── PUT api/alertes/{id}/resoudre ────────────────────────────
        [HttpPut("{id}/resoudre")]
        public IActionResult Resoudre(int id, [FromBody] CommentaireRequest req)
        {
            try
            {
                _alerteService.ResoudreAlerte(id, GetUserId(), req.Commentaire);
                return Ok(new { success = true, message = "Alerte résolue." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        }

        // ── PUT api/alertes/{id}/ignorer ─────────────────────────────
        [HttpPut("{id}/ignorer")]
        public IActionResult Ignorer(int id, [FromBody] CommentaireRequest req)
        {
            try
            {
                _alerteService.IgnorerAlerte(id, GetUserId(), req.Commentaire);
                return Ok(new { success = true, message = "Alerte ignorée." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        }

        // ── POST api/alertes/{id}/commentaires ───────────────────────
        [HttpPost("{id}/commentaires")]
        public IActionResult AjouterCommentaire(int id, [FromBody] NouveauCommentaireRequest req)
        {
            try
            {
                string nomAuteur = User.FindFirst("name")?.Value ?? "Utilisateur";
                _alerteService.AjouterCommentaire(id, GetUserId(), nomAuteur, req.Contenu);
                return Ok(new { success = true, message = "Commentaire ajouté." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }

        // ── GET api/alertes/{id}/commentaires ────────────────────────
        [HttpGet("{id}/commentaires")]
        public IActionResult GetCommentaires(int id)
        {
            var commentaires = _alerteService.GetCommentaires(id);
            return Ok(commentaires);
        }

        // ── POST api/alertes/verifier-seuils ─────────────────────────
        /// <summary>
        /// Déclenche manuellement la vérification des seuils.
        /// (En production, cette route devrait être réservée aux admins.)
        /// </summary>
        [HttpPost("verifier-seuils")]
        [Authorize(Roles = "Admin,Responsable")]
        public async Task<IActionResult> VerifierSeuils()
        {
            try
            {
                await _alerteService.VerifierSeuilsAsync();
                return Ok(new { success = true, message = "Vérification des seuils effectuée." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ── Helpers ──────────────────────────────────────────────────

        private int GetUserId()
        {
            var claim = User.FindFirst("id")?.Value;
            return int.TryParse(claim, out int id) ? id : 1;
        }
    }

    // ══════════════════════════════════════════════════════════════════
    // DTOs (séparés pour ne pas exposer les entités EF directement)
    // ══════════════════════════════════════════════════════════════════

    public record AlerteDto
    {
        public int Id { get; init; }
        public string CodeMachine { get; init; }
        public string? CodeArticle { get; init; }
        public string? NumOF { get; init; }
        public decimal TauxDetecte { get; init; }
        public int QuantiteDefauts { get; init; }
        public int QuantiteTotale { get; init; }
        public int NbNonConformesConsecutifs { get; init; }
        public string Niveau { get; init; }
        public string Statut { get; init; }
        public string Message { get; init; }
        public DateTime DateAlerte { get; init; }
        public DateTime? DateResolution { get; init; }

        public AlerteDto(Alerte a)
        {
            Id = a.Id;
            CodeMachine = a.CodeMachine;
            CodeArticle = a.CodeArticle;
            NumOF = a.NumOF;
            TauxDetecte = a.TauxDetecte;
            QuantiteDefauts = a.QuantiteDefauts;
            QuantiteTotale = a.QuantiteTotale;
            NbNonConformesConsecutifs = a.NbNonConformesConsecutifs;
            Niveau = a.Niveau.ToString();
            Statut = a.Statut.ToString();
            Message = a.Message;
            DateAlerte = a.DateAlerte;
            DateResolution = a.DateResolution;
        }
    }

    public record AlerteDetailDto : AlerteDto
    {
        public string? CommentaireResolution { get; init; }
        public IEnumerable<object> Commentaires { get; init; }

        public AlerteDetailDto(Alerte a) : base(a)
        {
            CommentaireResolution = a.CommentaireResolution;
            Commentaires = a.Commentaires
                .OrderByDescending(c => c.DateCreation)
                .Select(c => new
                {
                    c.Id,
                    c.NomAuteur,
                    c.Contenu,
                    c.DateCreation
                });
        }
    }

    public record CommentaireRequest(string Commentaire);
    public record NouveauCommentaireRequest(string Contenu);
}