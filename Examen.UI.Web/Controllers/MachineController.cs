using Microsoft.AspNetCore.Mvc;
using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;

namespace Examen.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MachineController : ControllerBase
    {
        private readonly IServiceMachine _serviceMachine;
        public MachineController(IServiceMachine serviceMachine)
        {
            _serviceMachine = serviceMachine;
        }

        [HttpGet]
        public IActionResult GetMachines()
        {
            var machines = _serviceMachine.GetAll();
            return Ok(machines);
        }

        [HttpGet("{code}")]
        public IActionResult GetMachine(string code)
        {
            var machine = _serviceMachine.GetById(code);
            if (machine == null)
                return NotFound();
            return Ok(machine);
        }

        [HttpPost]
        public IActionResult AddMachine([FromBody] Machine machine)
        {
            _serviceMachine.Add(machine);
            _serviceMachine.Commit();
            return CreatedAtAction(nameof(GetMachine),
                new { code = machine.CodeMachine }, machine);
        }

        [HttpPut("{code}")]
        public IActionResult UpdateMachine(string code, [FromBody] Machine machine)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var existing = _serviceMachine.GetById(code);
            if (existing == null)
                return NotFound();
            existing.CodeMachine = machine.CodeMachine;
            existing.NomMachine = machine.NomMachine;
            existing.Actif = machine.Actif;
            _serviceMachine.Update(existing);
            _serviceMachine.Commit();
            return NoContent();
        }

        [HttpDelete("{code}")]
        public IActionResult DeleteMachine(string code)
        {
            var existing = _serviceMachine.GetById(code);
            if (existing == null)
                return NotFound();

            _serviceMachine.Delete(existing);
            _serviceMachine.Commit();
            return NoContent();
        }

        [HttpPost("import-excel")]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Aucun fichier fourni" });

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (ext != ".xlsx" && ext != ".xls")
                return BadRequest(new { message = "Le fichier doit être un Excel (.xlsx, .xls)" });

            var errors = new List<string>();
            var ajoutees = 0;
            var ignorees = 0;

            try
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);

                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RowsUsed().Skip(1); // skip header

                foreach (var row in rows)
                {
                    var codeMachine = row.Cell(1).GetString().Trim();
                    var nomMachine = row.Cell(2).GetString().Trim();
                    var actifStr = row.Cell(3).GetString().Trim();

                    if (string.IsNullOrWhiteSpace(codeMachine) || string.IsNullOrWhiteSpace(nomMachine))
                    {
                        errors.Add($"Ligne {row.RowNumber()} : code ou nom machine manquant");
                        ignorees++;
                        continue;
                    }

                    if (_serviceMachine.GetById(codeMachine) != null)
                    {
                        errors.Add($"Ligne {row.RowNumber()} : la machine '{codeMachine}' existe déjà");
                        ignorees++;
                        continue;
                    }

                    bool actif = true;
                    if (!string.IsNullOrWhiteSpace(actifStr))
                    {
                        actif = actifStr.Equals("true", StringComparison.OrdinalIgnoreCase)
                                || actifStr.Equals("1")
                                || actifStr.Equals("oui", StringComparison.OrdinalIgnoreCase)
                                || actifStr.Equals("active", StringComparison.OrdinalIgnoreCase);
                    }

                    var machine = new Machine
                    {
                        CodeMachine = codeMachine,
                        NomMachine = nomMachine,
                        Actif = actif
                    };

                    _serviceMachine.Add(machine);
                    ajoutees++;
                }

                _serviceMachine.Commit();

                return Ok(new
                {
                    message = $"{ajoutees} machine(s) importée(s), {ignorees} ignorée(s)",
                    ajoutees,
                    ignorees,
                    erreurs = errors
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de l'import", detail = ex.Message, inner = ex.InnerException?.Message });
            }
        }
    }
}