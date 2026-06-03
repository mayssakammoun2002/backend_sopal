using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Examen.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AlertesController : ControllerBase
    {
        private readonly IServiceAlerte _svc;
        public AlertesController(IServiceAlerte svc) => _svc = svc;

        // GET /api/alertes
        [HttpGet]
        public IActionResult GetAll() =>
            Ok(_svc.GetAll().OrderByDescending(a => a.DateAlerte));

        // GET /api/alertes/actives
        [HttpGet("actives")]
        public IActionResult GetActives() => Ok(_svc.GetActives());

        // GET /api/alertes/{id}
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var a = _svc.GetById(id);
            return a == null ? NotFound(Msg($"Alerte {id} introuvable.")) : Ok(a);
        }

        // PUT /api/alertes/{id}/prendre-en-charge
        [HttpPut("{id:int}/prendre-en-charge")]
        public IActionResult PrendreEnCharge(int id)
        {
            try { _svc.PrendreEnChargeAlerte(id, UserId()); return Ok(Msg("Alerte prise en charge.")); }
            catch (KeyNotFoundException ex) { return NotFound(Msg(ex.Message)); }
            catch (InvalidOperationException ex) { return BadRequest(Msg(ex.Message)); }
        }

        // PUT /api/alertes/{id}/resoudre
        [HttpPut("{id:int}/resoudre")]
        public IActionResult Resoudre(int id, [FromBody] ActionDto dto)
        {
            try { _svc.ResoudreAlerte(id, UserId(), dto.Commentaire ?? ""); return Ok(Msg("Alerte résolue.")); }
            catch (KeyNotFoundException ex) { return NotFound(Msg(ex.Message)); }
            catch (InvalidOperationException ex) { return BadRequest(Msg(ex.Message)); }
        }

        // PUT /api/alertes/{id}/ignorer
        [HttpPut("{id:int}/ignorer")]
        public IActionResult Ignorer(int id, [FromBody] ActionDto dto)
        {
            try { _svc.IgnorerAlerte(id, UserId(), dto.Commentaire ?? ""); return Ok(Msg("Alerte ignorée.")); }
            catch (KeyNotFoundException ex) { return NotFound(Msg(ex.Message)); }
            catch (InvalidOperationException ex) { return BadRequest(Msg(ex.Message)); }
        }

        // GET /api/alertes/{id}/commentaires
        [HttpGet("{id:int}/commentaires")]
        public IActionResult GetCommentaires(int id)
        {
            if (_svc.GetById(id) == null) return NotFound(Msg($"Alerte {id} introuvable."));
            return Ok(_svc.GetCommentaires(id));
        }

        // POST /api/alertes/{id}/commentaires
        [HttpPost("{id:int}/commentaires")]
        public IActionResult AjouterCommentaire(int id, [FromBody] CommentaireDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Contenu))
                return BadRequest(Msg("Le contenu est obligatoire."));
            var nom = User.FindFirstValue(ClaimTypes.Name) ?? $"Utilisateur {UserId()}";
            try { _svc.AjouterCommentaire(id, UserId(), nom, dto.Contenu); return Ok(Msg("Commentaire ajouté.")); }
            catch (KeyNotFoundException ex) { return NotFound(Msg(ex.Message)); }
            catch (ArgumentException ex) { return BadRequest(Msg(ex.Message)); }
        }

        // DELETE /api/alertes/{id}  — Admin only
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var a = _svc.GetById(id);
            if (a == null) return NotFound(Msg($"Alerte {id} introuvable."));
            _svc.Delete(a); _svc.Commit();
            return NoContent();
        }

        private int UserId() =>
            int.TryParse(User.FindFirstValue("id"), out var id) ? id : 0;

        private static object Msg(string m) => new { message = m };
    }

    public record ActionDto(string? Commentaire);
    public record CommentaireDto(string Contenu);
}