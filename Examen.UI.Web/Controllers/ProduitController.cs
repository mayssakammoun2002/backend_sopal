using Microsoft.AspNetCore.Mvc;
using Examen.ApplicationCore.DTOs;
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
    public class ProduitController : ControllerBase
    {
        private readonly IServiceProduit _serviceProduit;

        public ProduitController(IServiceProduit serviceProduit)
        {
            _serviceProduit = serviceProduit;
        }

        [HttpGet]
        public IActionResult GetProduits()
        {
            return Ok(_serviceProduit.GetAll());
        }

        [HttpGet("{code}")]
        public IActionResult GetProduit(string code)
        {
            var produit = _serviceProduit.GetById(code?.Trim().ToUpper());
            if (produit == null)
                return NotFound(new
                {
                    message = "Produit non trouvé",
                    isUnknownProduct = true
                });

            return Ok(produit);
        }

        [HttpPost]
        public IActionResult AddProduit([FromBody] ProduitDTO dto)
        {
            try
            {
                if (dto == null || !ModelState.IsValid)
                    return BadRequest(ModelState);

                var codeArticle = (dto.CodeArticle ?? "").Trim().ToUpper();

                if (_serviceProduit.GetById(codeArticle) != null)
                    return BadRequest($"Le code {codeArticle} existe déjà.");

                var produit = new Produit
                {
                    CodeArticle = codeArticle,
                    NomProduit = (dto.NomProduit ?? "").Trim(),
                    Designation = (dto.Designation ?? "").Trim(),
                    TailleEchantillonnage = dto.TailleEchantillonnage
                };

                _serviceProduit.Add(produit);
                _serviceProduit.Commit();

                return Ok(new { success = true, message = "Produit ajouté avec succès", codeArticle });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, detail = ex.InnerException?.Message });
            }
        }

        [HttpDelete("{code}")]
        public IActionResult DeleteProduit(string code)
        {
            try
            {
                var produit = _serviceProduit.GetById(code?.Trim().ToUpper());

                if (produit == null)
                    return NotFound($"Produit avec le code {code} introuvable.");

                _serviceProduit.Delete(produit);
                _serviceProduit.Commit();

                return Ok(new
                {
                    success = true,
                    message = "Produit supprimé avec succès"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erreur lors de la suppression",
                    detail = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
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
            var produitsAjoutes = 0;
            var produitsIgnores = 0;

            try
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);

                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RowsUsed().Skip(1); // skip header

                foreach (var row in rows)
                {
                    var codeArticle = row.Cell(1).GetString().Trim();
                    var nomProduit = row.Cell(2).GetString().Trim();
                    var designation = row.Cell(3).GetString().Trim();
                    var tailleStr = row.Cell(4).GetString().Trim();

                    if (string.IsNullOrWhiteSpace(codeArticle) || string.IsNullOrWhiteSpace(nomProduit))
                    {
                        errors.Add($"Ligne {row.RowNumber()} : code article ou nom produit manquant");
                        produitsIgnores++;
                        continue;
                    }

                    codeArticle = codeArticle.ToUpperInvariant();

                    if (_serviceProduit.GetById(codeArticle) != null)
                    {
                        errors.Add($"Ligne {row.RowNumber()} : le produit '{codeArticle}' existe déjà");
                        produitsIgnores++;
                        continue;
                    }

                    int.TryParse(tailleStr, out var taille);

                    var produit = new Produit
                    {
                        CodeArticle = codeArticle,
                        NomProduit = nomProduit,
                        Designation = designation,
                        TailleEchantillonnage = taille
                    };

                    _serviceProduit.Add(produit);
                    produitsAjoutes++;
                }

                _serviceProduit.Commit();

                return Ok(new
                {
                    message = $"{produitsAjoutes} produit(s) importé(s), {produitsIgnores} ignoré(s)",
                    ajoutes = produitsAjoutes,
                    ignores = produitsIgnores,
                    erreurs = errors
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de l'import", detail = ex.Message, inner = ex.InnerException?.Message });
            }
        }

        [HttpPut("{code}")]
        public IActionResult UpdateProduit(string code, [FromBody] ProduitDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var produit = _serviceProduit.GetById(code);
                if (produit == null)
                    return NotFound($"Produit avec le code {code} introuvable.");

                produit.NomProduit = (dto.NomProduit ?? produit.NomProduit)?.Trim() ?? produit.NomProduit;
                produit.Designation = (dto.Designation ?? produit.Designation)?.Trim() ?? produit.Designation;
                produit.TailleEchantillonnage = dto.TailleEchantillonnage;

                _serviceProduit.Update(produit);
                _serviceProduit.Commit();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message, Inner = ex.InnerException?.Message });
            }
        }
    }
}