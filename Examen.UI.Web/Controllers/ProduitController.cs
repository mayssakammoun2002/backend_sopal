using Microsoft.AspNetCore.Mvc;
using Examen.ApplicationCore.DTOs;
using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.Interfaces;
using System;
using System.Linq;

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

                // ❌ Supprimé : TypeDefaut n'a plus de relation avec Produit

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

                // ❌ Supprimé : TypeDefaut n'a plus de relation avec Produit

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