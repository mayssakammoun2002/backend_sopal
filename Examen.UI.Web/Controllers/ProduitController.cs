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

                // Vérification doublon
                if (_serviceProduit.GetById(dto.CodeArticle?.Trim().ToUpper()) != null)
                {
                    return BadRequest($"Un produit avec le code '{dto.CodeArticle}' existe déjà.");
                }

                var produit = new Produit
                {
                    CodeArticle = (dto.CodeArticle ?? "").Trim().ToUpper(),
                    NomProduit = (dto.NomProduit ?? "").Trim(),
                    Designation = (dto.Designation ?? "").Trim(),
                    TailleEchantillonnage = dto.TailleEchantillonnage,
                    TypeDefauts = new List<TypeDefaut>()
                };

                // Ajout des défauts
                if (dto.TypeDefautIds?.Any() == true)
                {
                    var defauts = _serviceProduit.GetAllTypeDefauts()
                        .Where(d => dto.TypeDefautIds.Contains(d.Id))
                        .ToList();

                    if (defauts.Count != dto.TypeDefautIds.Count)
                        return BadRequest("Un ou plusieurs types de défauts n'existent pas.");

                    foreach (var defaut in defauts)
                    {
                        produit.TypeDefauts.Add(defaut);
                    }
                }

                _serviceProduit.Add(produit);
                _serviceProduit.Commit();

                // 🔥 SOLUTION : On retourne uniquement les infos nécessaires (pas l'entité complète)
                return Ok(new
                {
                    success = true,
                    message = "Produit ajouté avec succès !",
                    codeArticle = produit.CodeArticle
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("=== ERREUR AddProduit ===\n" + ex.ToString());

                return StatusCode(500, new
                {
                    success = false,
                    message = "Erreur lors de l'ajout du produit",
                    detail = ex.Message,
                    inner = ex.InnerException?.Message
                });
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
                Console.WriteLine("=== ERREUR DeleteProduit ===\n" + ex.ToString());

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

                // Mise à jour des propriétés (sécurisée)
                produit.NomProduit = (dto.NomProduit ?? produit.NomProduit)?.Trim() ?? produit.NomProduit;
                produit.Designation = (dto.Designation ?? produit.Designation)?.Trim() ?? produit.Designation;
                produit.TailleEchantillonnage = dto.TailleEchantillonnage;

                // Mise à jour des TypeDefauts (remplace tout)
                if (dto.TypeDefautIds != null)
                {
                    produit.TypeDefauts.Clear();   // Supprime les anciennes relations

                    if (dto.TypeDefautIds.Any())
                    {
                        var defautsExistants = _serviceProduit.GetAllTypeDefauts()
                            .Where(d => dto.TypeDefautIds.Contains(d.Id))
                            .ToList();

                        if (defautsExistants.Count != dto.TypeDefautIds.Count)
                            return BadRequest("Un ou plusieurs TypeDefautIds sont invalides.");

                        foreach (var defaut in defautsExistants)
                        {
                            produit.TypeDefauts.Add(defaut);
                        }
                    }
                }

                _serviceProduit.Update(produit);
                _serviceProduit.Commit();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine("=== ERREUR UpdateProduit ===\n" + ex.ToString());
                return StatusCode(500, new { Message = ex.Message, Inner = ex.InnerException?.Message });
            }
        }
    }
}