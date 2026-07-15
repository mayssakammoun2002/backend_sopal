using System;
using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.DTOs.Common;
using Examen.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Examen.UI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "RequireAdmin")]
    public class MachineController : ControllerBase
    {
        private readonly IServiceMachine _serviceMachine;

        public MachineController(IServiceMachine serviceMachine)
        {
            _serviceMachine = serviceMachine;
        }

        // GET /api/Machine  → liste paginée + recherche
        [HttpGet]
        public async Task<IActionResult> GetMachines([FromQuery] PaginationParams paginationParams)
        {
            var resultat = await _serviceMachine.GetMachinesPagineesAsync(paginationParams);
            return Ok(resultat);
        }

        [HttpGet("{codeMachine}")]
        public IActionResult GetById(string codeMachine)
        {
            try
            {
                var machine = _serviceMachine.GetById(codeMachine);
                if (machine == null)
                    return NotFound(new { message = $"Machine {codeMachine} introuvable." });

                return Ok(machine);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, detail = ex.InnerException?.Message });
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] Machine machine)
        {
            try
            {
                if (machine == null || !ModelState.IsValid)
                    return BadRequest(ModelState);

                _serviceMachine.Add(machine);
                _serviceMachine.Commit();

                return CreatedAtAction(nameof(GetById), new { codeMachine = machine.CodeMachine }, machine);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, detail = ex.InnerException?.Message });
            }
        }

        [HttpPut("{codeMachine}")]
        public IActionResult Update(string codeMachine, [FromBody] Machine machine)
        {
            try
            {
                var existant = _serviceMachine.GetById(codeMachine);
                if (existant == null)
                    return NotFound(new { message = $"Machine {codeMachine} introuvable." });

                existant.NomMachine = machine.NomMachine;
                existant.Actif = machine.Actif;

                _serviceMachine.Update(existant);
                _serviceMachine.Commit();

                return Ok(existant);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, detail = ex.InnerException?.Message });
            }
        }

        [HttpDelete("{codeMachine}")]
        public IActionResult Delete(string codeMachine)
        {
            try
            {
                var existant = _serviceMachine.GetById(codeMachine);
                if (existant == null)
                    return NotFound(new { message = $"Machine {codeMachine} introuvable." });

                _serviceMachine.DeleteById(codeMachine);
                _serviceMachine.Commit();

                return Ok(new { success = true, message = "Machine supprimée avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, detail = ex.InnerException?.Message });
            }
        }

        // ⚠️ Reprends ici ton implémentation existante d'import Excel (ExcelReader, mapping colonnes, etc.)
        [HttpPost("import-excel")]
        public IActionResult ImportExcel(IFormFile file)
        {
            // Garde ta logique d'import existante, non reconstruite ici faute du fichier original.
            return StatusCode(501, new { message = "À compléter avec ta logique d'import existante." });
        }
    }
}