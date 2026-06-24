using Microsoft.AspNetCore.Mvc;
using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.Interfaces;
using Examen.ApplicationCore.DTO;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
namespace Examen.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TypeDefautController : ControllerBase
    {
        private readonly IServiceTypeDefaut _serviceTypeDefaut;

        public TypeDefautController(IServiceTypeDefaut serviceTypeDefaut)
        {
            _serviceTypeDefaut = serviceTypeDefaut;
        }

        // GET: api/TypeDefaut
        [HttpGet]
        public IActionResult GetAll()
        {
            var types = _serviceTypeDefaut.GetAll();
            return Ok(types);
        }

        // GET: api/TypeDefaut/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var typeDefaut = _serviceTypeDefaut.GetById(id);
            if (typeDefaut == null)
                return NotFound();

            return Ok(typeDefaut);
        }

        // POST: api/TypeDefaut
        [HttpPost]
        [Consumes("multipart/form-data")]
        public IActionResult Add([FromForm] TypeDefautDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string imageName = string.Empty;

            // Upload image
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                if (!Directory.Exists(imagesFolder))
                    Directory.CreateDirectory(imagesFolder);

                imageName = $"{Guid.NewGuid()}{Path.GetExtension(dto.ImageFile.FileName)}";
                var path = Path.Combine(imagesFolder, imageName);

                using var stream = new FileStream(path, FileMode.Create);
                dto.ImageFile.CopyTo(stream);
            }

            var type = new TypeDefaut
            {
                NomDefaut = dto.NomDefaut,
                Description = dto.Description,
                CauseProbable = dto.CauseProbable, // ✅ AJOUT
                Solution = dto.Solution,
                Frequence = dto.Frequence,
                ImagePath = imageName
            };

            _serviceTypeDefaut.Add(type);
            _serviceTypeDefaut.Commit();

            return CreatedAtAction(nameof(GetById), new { id = type.Id }, type);
        }

        // PUT: api/TypeDefaut/5
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public IActionResult Update(int id, [FromForm] TypeDefautDto dto)
        {
            var existing = _serviceTypeDefaut.GetById(id);
            if (existing == null)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            existing.NomDefaut = dto.NomDefaut;
            existing.Description = dto.Description;
            existing.CauseProbable = dto.CauseProbable; // ✅ AJOUT
            existing.Solution = dto.Solution;
            existing.Frequence = dto.Frequence;

            // Update image
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                if (!Directory.Exists(imagesFolder))
                    Directory.CreateDirectory(imagesFolder);

                var imageName = $"{Guid.NewGuid()}{Path.GetExtension(dto.ImageFile.FileName)}";
                var path = Path.Combine(imagesFolder, imageName);

                using var stream = new FileStream(path, FileMode.Create);
                dto.ImageFile.CopyTo(stream);

                existing.ImagePath = imageName;
            }

            _serviceTypeDefaut.Update(existing);
            _serviceTypeDefaut.Commit();

            return Ok(existing);
        }
        // POST: api/TypeDefaut/import-excel
        [HttpPost("import-excel")]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Aucun fichier fourni" });

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (ext != ".xlsx" && ext != ".xls")
                return BadRequest(new { message = "Le fichier doit être un Excel (.xlsx, .xls)" });

            var errors = new List<string>();
            var ajoutes = 0;
            var ignores = 0;

            try
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);

                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RowsUsed().Skip(1); // skip header

                foreach (var row in rows)
                {
                    var nomDefaut = row.Cell(1).GetString().Trim();
                    var description = row.Cell(2).GetString().Trim();
                    var causeProbable = row.Cell(3).GetString().Trim();
                    var solution = row.Cell(4).GetString().Trim();
                    var frequenceStr = row.Cell(5).GetString().Trim();

                    if (string.IsNullOrWhiteSpace(nomDefaut))
                    {
                        errors.Add($"Ligne {row.RowNumber()} : nom du défaut manquant");
                        ignores++;
                        continue;
                    }

                    int.TryParse(frequenceStr, out var frequence);

                    var type = new TypeDefaut
                    {
                        NomDefaut = nomDefaut,
                        Description = description,
                        CauseProbable = causeProbable,
                        Solution = solution,
                        Frequence = frequence,
                        ImagePath = string.Empty
                    };

                    _serviceTypeDefaut.Add(type);
                    ajoutes++;
                }

                _serviceTypeDefaut.Commit();

                return Ok(new
                {
                    message = $"{ajoutes} défaut(s) importé(s), {ignores} ignoré(s)",
                    ajoutes,
                    ignores,
                    erreurs = errors
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de l'import", detail = ex.Message, inner = ex.InnerException?.Message });
            }
        }
        // DELETE: api/TypeDefaut/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var typeDefaut = _serviceTypeDefaut.GetById(id);
            if (typeDefaut == null)
                return NotFound();

            _serviceTypeDefaut.Delete(typeDefaut);
            _serviceTypeDefaut.Commit();

            return NoContent();
        }
    }
}