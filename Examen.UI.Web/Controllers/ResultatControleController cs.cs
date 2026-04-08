using Microsoft.AspNetCore.Mvc;
using Examen.ApplicationCore.DTOs;
using Examen.ApplicationCore.Interfaces;
using System;

namespace Examen.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultatControleController : ControllerBase
    {
        private readonly IServiceResultatControle _service;

        public ResultatControleController(IServiceResultatControle service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var resultats = _service.GetAll();
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
                    // Ceci te montre exactement quels champs posent problème
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    return BadRequest(new
                    {
                        success = false,
                        message = "Données invalides",
                        errors
                    });
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