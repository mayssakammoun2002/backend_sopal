using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Examen.ApplicationCore.DTOs;
using Examen.ApplicationCore.DTOs.Common;
using Examen.ApplicationCore.Interfaces;
using System;
using System.Linq;
using System.Security.Claims;

namespace Examen.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ResultatControleController : ControllerBase
    {
        private readonly IServiceResultatControle _service;

        public ResultatControleController(IServiceResultatControle service)
        {
            _service = service;
        }

        private int? GetUtilisateurIdConnecte()
        {
            var claim = User.FindFirst("id")?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }

        private bool EstAdmin()
        {
            var claim = User.FindFirst("estAdmin")?.Value;
            return claim == "true";
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] PaginationParams paginationParams)
        {
            try
            {
                var resultats = _service.GetAllPaginated(
                    GetUtilisateurIdConnecte(), EstAdmin(), paginationParams);
                return Ok(resultats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Erreur lors de la récupération des contrôles",
                    detail = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            try
            {
                var resultat = _service.GetById(id);
                return Ok(resultat);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Contrôle introuvable" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Add([FromBody] ResultatControleDTO dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { success = false, message = "Le corps de la requête est vide" });

                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    return BadRequest(new { success = false, message = "Données invalides", errors });
                }

                var resultat = _service.Ajouter(dto);

                return Ok(new
                {
                    success = true,
                    id = resultat.Id,
                    statutLot = resultat.StatutLot,
                    message = "Contrôle qualité enregistré avec succès"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody] ResultatControleDTO dto)
        {
            try
            {
                if (dto == null || !ModelState.IsValid)
                    return BadRequest(ModelState);

                var resultat = _service.Modifier(id, dto);

                return Ok(new
                {
                    success = true,
                    id = resultat.Id,
                    statutLot = resultat.StatutLot,
                    message = "Contrôle qualité modifié avec succès"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Contrôle introuvable" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("stats")]
        public IActionResult GetStats(
            [FromQuery] string? codeMachine,
            [FromQuery] string? statut,
            [FromQuery] DateTime? dateDebut,
            [FromQuery] DateTime? dateFin)
        {
            try
            {
                var stats = _service.GetStats(
                    codeMachine, statut, dateDebut, dateFin,
                    GetUtilisateurIdConnecte(), EstAdmin());

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Erreur lors du calcul des statistiques",
                    detail = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            try
            {
                _service.Delete(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Contrôle introuvable" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}