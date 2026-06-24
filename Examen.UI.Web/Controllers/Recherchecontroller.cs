using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Examen.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RechercheController : ControllerBase
{
    private readonly IServiceProduit _serviceProduit;
    private readonly IServiceMachine _serviceMachine;
    private readonly IServiceLot _serviceLot;
    private readonly IServiceTypeDefaut _serviceTypeDefaut;
    private readonly IServiceResultatControle _serviceResultatControle;
    private readonly IServiceUtilisateur _serviceUtilisateur;

    public RechercheController(
        IServiceProduit serviceProduit,
        IServiceMachine serviceMachine,
        IServiceLot serviceLot,
        IServiceTypeDefaut serviceTypeDefaut,
        IServiceResultatControle serviceResultatControle,
        IServiceUtilisateur serviceUtilisateur)
    {
        _serviceProduit = serviceProduit;
        _serviceMachine = serviceMachine;
        _serviceLot = serviceLot;
        _serviceTypeDefaut = serviceTypeDefaut;
        _serviceResultatControle = serviceResultatControle;
        _serviceUtilisateur = serviceUtilisateur;
    }

    // Récupère l'Id de l'utilisateur connecté à partir du JWT (claim "id")
    private int? GetUtilisateurIdConnecte()
    {
        var claim = User.FindFirst("id")?.Value;
        return int.TryParse(claim, out var id) ? id : null;
    }

    /// <summary>
    /// Recherche globale unifiée sur Produits, Machines, Lots, Types de défauts,
    /// Résultats de contrôle et Utilisateurs (ces derniers réservés aux Admin).
    /// GET /api/Recherche?q=...&take=5
    /// </summary>
    [HttpGet]
    public IActionResult Search([FromQuery] string q, [FromQuery] int take = 5)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Trim().Length < 2)
        {
            return Ok(new
            {
                produits = Array.Empty<object>(),
                machines = Array.Empty<object>(),
                lots = Array.Empty<object>(),
                defauts = Array.Empty<object>(),
                controles = Array.Empty<object>(),
                utilisateurs = Array.Empty<object>()
            });
        }

        var term = q.Trim();
        var isAdmin = User.IsInRole("Admin");
        var utilisateurIdConnecte = GetUtilisateurIdConnecte();

        var produits = _serviceProduit.GetAll()
            .Where(p =>
                Contains(p.CodeArticle, term) ||
                Contains(p.NomProduit, term) ||
                Contains(p.Designation, term))
            .Take(take)
            .Select(p => new
            {
                type = "Produit",
                id = p.CodeArticle,
                titre = p.NomProduit,
                sousTitre = p.CodeArticle,
                route = "/form_litedesproduits"
            });

        var machines = _serviceMachine.GetAll()
            .Where(m =>
                Contains(m.CodeMachine, term) ||
                Contains(m.NomMachine, term))
            .Take(take)
            .Select(m => new
            {
                type = "Machine",
                id = m.CodeMachine,
                titre = m.NomMachine,
                sousTitre = m.CodeMachine,
                route = "/form_crudmachine"
            });

        var lots = _serviceLot.GetAll()
            .Where(l =>
                Contains(l.NumeroLot, term) ||
                Contains(l.MachineId, term) ||
                Contains(l.ProduitId, term))
            .Take(take)
            .Select(l => new
            {
                type = "Lot",
                id = l.Id.ToString(),
                titre = l.NumeroLot,
                sousTitre = $"{l.MachineId} — {l.ProduitId}",
                route = "/lots"
            });

        var defauts = _serviceTypeDefaut.GetAll()
            .Where(d =>
                Contains(d.NomDefaut, term) ||
                Contains(d.Description, term))
            .Take(take)
            .Select(d => new
            {
                type = "TypeDefaut",
                id = d.Id.ToString(),
                titre = d.NomDefaut,
                sousTitre = Truncate(d.Description, 60),
                route = "/form_crudtypedefaut"
            });

        // Un opérateur ne retrouve dans la recherche que ses propres contrôles ; l'admin retrouve tout
        var controles = _serviceResultatControle.GetAll(utilisateurIdConnecte, isAdmin)
            .Where(c =>
                Contains(c.NumOF, term) ||
                Contains(c.CodeArticle, term) ||
                Contains(c.NomProduit, term) ||
                Contains(c.CodeMachine, term) ||
                Contains(c.NomMachine, term) ||
                Contains(c.NumLotMatiere, term) ||
                Contains(c.Controleur, term))
            .Take(take)
            .Select(c => new
            {
                type = "ResultatControle",
                id = c.Id,
                titre = $"OF {c.NumOF}",
                sousTitre = $"{c.NomProduit} — {c.NomMachine} — {c.StatutLot}",
                route = "/form_resultat_de_controle"
            });

        // Recherche sur les utilisateurs : réservée aux Admin (même règle que UtilisateurController.GetAll)
        IEnumerable<object> utilisateurs = Array.Empty<object>();
        if (isAdmin)
        {
            utilisateurs = _serviceUtilisateur.GetAll()
                .Where(u =>
                    Contains(u.FirstName, term) ||
                    Contains(u.LastName, term) ||
                    Contains(u.Email, term))
                .Take(take)
                .Select(u => new
                {
                    type = "Utilisateur",
                    id = u.Id.ToString(),
                    titre = $"{u.FirstName} {u.LastName}",
                    sousTitre = u.Email,
                    route = "/form_crudUser"
                });
        }

        return Ok(new
        {
            produits,
            machines,
            lots,
            defauts,
            controles,
            utilisateurs
        });
    }

    private static bool Contains(string? source, string term)
    {
        if (string.IsNullOrEmpty(source)) return false;
        return source.Contains(term, StringComparison.OrdinalIgnoreCase);
    }

    private static string Truncate(string? text, int max)
    {
        if (string.IsNullOrEmpty(text)) return "";
        return text.Length > max ? text.Substring(0, max) + "..." : text;
    }
}