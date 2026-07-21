using System;
using System.Globalization;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
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
    public class ProduitController : ControllerBase
    {
        private readonly IServiceProduit _serviceProduit;
        private static readonly CultureInfo CultureFr = new CultureInfo("fr-FR");

        public ProduitController(IServiceProduit serviceProduit)
        {
            _serviceProduit = serviceProduit;
        }

        // GET /api/Produit  → liste paginée + recherche
        [HttpGet]
        public async Task<IActionResult> GetProduits([FromQuery] PaginationParams paginationParams)
        {
            var resultat = await _serviceProduit.GetProduitsPaginesAsync(paginationParams);
            return Ok(resultat);
        }

        [HttpGet("{codeArticle}")]
        public IActionResult GetById(string codeArticle)
        {
            try
            {
                var produit = _serviceProduit.GetById(codeArticle);
                if (produit == null)
                    return NotFound(new { message = $"Produit {codeArticle} introuvable." });

                return Ok(produit);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, detail = ex.InnerException?.Message });
            }
        }

        [HttpPost]
        [Authorize(Policy = "RequireAdmin")]
        public IActionResult Create([FromBody] ProduitDTO dto)
        {
            try
            {
                if (dto == null || !ModelState.IsValid)
                    return BadRequest(ModelState);

                var produit = new Produit
                {
                    CodeArticle = dto.CodeArticle,
                    NomProduit = dto.NomProduit,
                    Designation = dto.Designation,
                    TailleEchantillonnage = dto.TailleEchantillonnage
                };

                _serviceProduit.Add(produit);
                _serviceProduit.Commit();

                return CreatedAtAction(nameof(GetById), new { codeArticle = produit.CodeArticle }, produit);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, detail = ex.InnerException?.Message });
            }
        }

        [HttpPut("{codeArticle}")]
        [Authorize(Policy = "RequireAdmin")]
        public IActionResult Update(string codeArticle, [FromBody] ProduitDTO dto)
        {
            try
            {
                var existant = _serviceProduit.GetById(codeArticle);
                if (existant == null)
                    return NotFound(new { message = $"Produit {codeArticle} introuvable." });

                existant.NomProduit = dto.NomProduit;
                existant.Designation = dto.Designation;
                existant.TailleEchantillonnage = dto.TailleEchantillonnage;

                _serviceProduit.Update(existant);
                _serviceProduit.Commit();

                return Ok(existant);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, detail = ex.InnerException?.Message });
            }
        }

        [HttpDelete("{codeArticle}")]
        [Authorize(Policy = "RequireAdmin")]
        public IActionResult Delete(string codeArticle)
        {
            try
            {
                var existant = _serviceProduit.GetById(codeArticle);
                if (existant == null)
                    return NotFound(new { message = $"Produit {codeArticle} introuvable." });

                _serviceProduit.DeleteById(codeArticle);
                _serviceProduit.Commit();

                return Ok(new { success = true, message = "Produit supprimé avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, detail = ex.InnerException?.Message });
            }
        }

        // POST /api/Produit/import-excel
        // Colonnes attendues (ligne 1 = en-têtes, lecture à partir de la ligne 2) :
        // A: CodeArticle | B: NomProduit | C: Designation | D: TailleEchantillonnage
        // Cadence n'est pas dans le fichier : valeur par défaut = 1 (à ajuster si besoin)
        [HttpPost("import-excel")]
        [Authorize(Policy = "RequireAdmin")]
        public IActionResult ImportExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Aucun fichier reçu." });

            var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            if (extension != ".xlsx" && extension != ".xlsm")
                return BadRequest(new { message = "Format de fichier invalide. Seuls les fichiers .xlsx/.xlsm sont acceptés." });

            const int cadenceParDefaut = 1;

            var produitsAjoutes = 0;
            var produitsIgnores = 0;
            var erreurs = new System.Collections.Generic.List<string>();

            try
            {
                using var stream = new MemoryStream();
                file.CopyTo(stream);
                stream.Position = 0;

                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return BadRequest(new { message = "Le fichier Excel ne contient aucune feuille." });

                var derniereLigne = worksheet.LastRowUsed()?.RowNumber() ?? 1;

                for (int ligne = 2; ligne <= derniereLigne; ligne++)
                {
                    var row = worksheet.Row(ligne);

                    var codeArticle = row.Cell(1).GetString().Trim();
                    var nomProduit = row.Cell(2).GetString().Trim();
                    var designation = row.Cell(3).GetString().Trim();
                    var tailleEchantillonnageRaw = row.Cell(4).GetString().Trim();

                    if (string.IsNullOrWhiteSpace(codeArticle))
                        continue;

                    if (_serviceProduit.GetById(codeArticle) != null)
                    {
                        produitsIgnores++;
                        erreurs.Add($"Ligne {ligne} : le produit {codeArticle} existe déjà, ignoré.");
                        continue;
                    }

                    if (!decimal.TryParse(
                            tailleEchantillonnageRaw,
                            NumberStyles.Any,
                            CultureFr,
                            out var tailleDecimal))
                    {
                        produitsIgnores++;
                        erreurs.Add($"Ligne {ligne} : TailleEchantillonnage invalide ('{tailleEchantillonnageRaw}').");
                        continue;
                    }

                    var tailleEchantillonnage = (int)Math.Round(tailleDecimal);

                    if (tailleEchantillonnage < 1 || tailleEchantillonnage > 1000)
                    {
                        produitsIgnores++;
                        erreurs.Add($"Ligne {ligne} : TailleEchantillonnage hors plage (1-1000) : {tailleEchantillonnage}.");
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(nomProduit) || nomProduit.Length > 50)
                    {
                        produitsIgnores++;
                        erreurs.Add($"Ligne {ligne} : NomProduit invalide ou trop long (max 50 caractères).");
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(designation) || designation.Length < 3 || designation.Length > 100)
                    {
                        produitsIgnores++;
                        erreurs.Add($"Ligne {ligne} : Designation invalide (doit contenir entre 3 et 100 caractères).");
                        continue;
                    }

                    var produit = new Produit
                    {
                        CodeArticle = codeArticle,
                        NomProduit = nomProduit,
                        Designation = designation,
                        TailleEchantillonnage = tailleEchantillonnage,
                        Cadence = cadenceParDefaut
                    };

                    _serviceProduit.Add(produit);
                    produitsAjoutes++;
                }

                _serviceProduit.Commit();

                return Ok(new
                {
                    success = true,
                    message = $"{produitsAjoutes} produit(s) importé(s), {produitsIgnores} ignoré(s).",
                    produitsAjoutes,
                    produitsIgnores,
                    erreurs
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de l'import Excel.", detail = ex.Message, inner = ex.InnerException?.Message });
            }
        }
    }
}