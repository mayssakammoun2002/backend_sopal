using System;
using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.DTO;
using Examen.ApplicationCore.DTOs.Common;
using Examen.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Examen.UI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TypeDefautController : ControllerBase
    {
        private readonly IServiceTypeDefaut _serviceTypeDefaut;

        public TypeDefautController(IServiceTypeDefaut serviceTypeDefaut)
        {
            _serviceTypeDefaut = serviceTypeDefaut;
        }

        [HttpGet]
        public async Task<IActionResult> GetTypeDefauts([FromQuery] PaginationParams paginationParams)
        {
            var resultat = await _serviceTypeDefaut.GetTypeDefautsPaginesAsync(paginationParams);
            return Ok(resultat);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var defaut = _serviceTypeDefaut.GetById(id);
                if (defaut == null)
                    return NotFound(new { message = $"Défaut {id} introuvable." });

                return Ok(defaut);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, detail = ex.InnerException?.Message });
            }
        }

        [HttpPost]
        [Authorize(Policy = "RequireAdmin")]
        public IActionResult Create([FromForm] TypeDefautDto dto)
        {
            try
            {
                if (dto == null || !ModelState.IsValid)
                    return BadRequest(ModelState);

                var defaut = new TypeDefaut
                {
                    NomDefaut = dto.NomDefaut,
                    Description = dto.Description,
                    CauseProbable = dto.CauseProbable,
                    Solution = dto.Solution,
                    Frequence = dto.Frequence
                };

                _serviceTypeDefaut.Add(defaut);
                _serviceTypeDefaut.Commit();

                return CreatedAtAction(nameof(GetById), new { id = defaut.Id }, defaut);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, detail = ex.InnerException?.Message });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "RequireAdmin")]
        public IActionResult Update(int id, [FromForm] TypeDefautDto dto)
        {
            try
            {
                var existant = _serviceTypeDefaut.GetById(id);
                if (existant == null)
                    return NotFound(new { message = $"Défaut {id} introuvable." });

                existant.NomDefaut = dto.NomDefaut;
                existant.Description = dto.Description;
                existant.CauseProbable = dto.CauseProbable;
                existant.Solution = dto.Solution;
                existant.Frequence = dto.Frequence;

                _serviceTypeDefaut.Update(existant);
                _serviceTypeDefaut.Commit();

                return Ok(existant);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, detail = ex.InnerException?.Message });
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "RequireAdmin")]
        public IActionResult Delete(int id)
        {
            try
            {
                var existant = _serviceTypeDefaut.GetById(id);
                if (existant == null)
                    return NotFound(new { message = $"Défaut {id} introuvable." });

                _serviceTypeDefaut.DeleteById(id);
                _serviceTypeDefaut.Commit();

                return Ok(new { success = true, message = "Défaut supprimé avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, detail = ex.InnerException?.Message });
            }
        }

        [HttpPost("import-excel")]
        [Authorize(Policy = "RequireAdmin")]
        public IActionResult ImportExcel(IFormFile file)
        {
            return StatusCode(501, new { message = "À compléter avec ta logique d'import existante." });
        }
    }
}