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
        private readonly IServiceAlerte _alerteService;

        public AlertesController(IServiceAlerte alerteService)
        {
            _alerteService = alerteService;
        }

        // GET /api/alertes
        [HttpGet]
        public IActionResult GetAll()
        {
            var alertes = _alerteService.GetAll().ToList();
            return Ok(alertes);
        }

        // GET /api/alertes/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var alerte = _alerteService.GetById(id);
            if (alerte == null)
                return NotFound(new { message = $"Alerte {id} introuvable" });

            return Ok(alerte);
        }

        // GET /api/alertes/actives
        [HttpGet("actives")]
        public IActionResult GetActives()
        {
            var alertes = _alerteService.GetAll()
                .Where(a => a.Statut == StatutAlerte.Nouvelle || a.Statut == StatutAlerte.EnCours)
                .OrderByDescending(a => a.DateAlerte)
                .ToList();

            return Ok(alertes);
        }

        // PUT /api/alertes/{id}/resoudre
        [HttpPut("{id}/resoudre")]
        public IActionResult Resoudre(int id, [FromBody] ResoudreAlerteDto dto)
        {
            var userId = int.Parse(User.FindFirstValue("id") ?? "0");

            try
            {
                _alerteService.ResoudreAlerte(id, userId, dto.Commentaire ?? "");
                return Ok(new { message = "Alerte résolue avec succès" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE /api/alertes/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var alerte = _alerteService.GetById(id);
            if (alerte == null)
                return NotFound(new { message = $"Alerte {id} introuvable" });

            _alerteService.Delete(alerte);
            _alerteService.Commit();
            return NoContent();
        }
    }

    public class ResoudreAlerteDto
    {
        public string? Commentaire { get; set; }
    }
}