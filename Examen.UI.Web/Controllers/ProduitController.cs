using System;
using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.DTOs;
using Examen.ApplicationCore.DTOs.Common;
using Examen.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Examen.UI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "RequireAdmin")]
    public class ProduitController : ControllerBase
    {
        private readonly IServiceProduit _serviceProduit;

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

        // ⚠️ Reprends ici ton implémentation existante d'import Excel
        [HttpPost("import-excel")]
        public IActionResult ImportExcel(IFormFile file)
        {
            return StatusCode(501, new { message = "À compléter avec ta logique d'import existante." });
        }
    }
}