using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.DTOs;
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
    public class MachineController : ControllerBase
    {
        private readonly IServiceMachine _serviceMachine;

        public MachineController(IServiceMachine serviceMachine)
        {
            _serviceMachine = serviceMachine;
        }

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
        [Authorize(Policy = "RequireAdmin")]
        public IActionResult Create([FromBody] MachineCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var existant = _serviceMachine.GetById(dto.CodeMachine);
                if (existant != null)
                    return BadRequest(new { message = $"La machine {dto.CodeMachine} existe déjà." });

                var machine = new Machine
                {
                    CodeMachine = dto.CodeMachine,
                    NomMachine = dto.NomMachine,
                    Actif = dto.Actif,
                    ResultatControles = new List<ResultatControle>(),
                    Lots = new List<Lot>()
                };

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
        [Authorize(Policy = "RequireAdmin")]
        public IActionResult Update(string codeMachine, [FromBody] MachineUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var existant = _serviceMachine.GetById(codeMachine);
                if (existant == null)
                    return NotFound(new { message = $"Machine {codeMachine} introuvable." });

                existant.NomMachine = dto.NomMachine;
                existant.Actif = dto.Actif;

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
        [Authorize(Policy = "RequireAdmin")]
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

        [HttpPost("import-excel")]
        [Authorize(Policy = "RequireAdmin")]
        public IActionResult ImportExcel(IFormFile file)
        {
            return StatusCode(501, new { message = "À compléter avec ta logique d'import existante." });
        }
    }
}