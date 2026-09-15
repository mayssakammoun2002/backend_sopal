using System.Security.Claims;
using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.DTOs;
using Examen.ApplicationCore.DTOs.Common;
using Examen.ApplicationCore.Interfaces;
using Examen.ApplicationCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class UtilisateurController : ControllerBase
{
    private readonly IServiceUtilisateur _service;
    private readonly JwtService _jwtService;

    public UtilisateurController(
        IServiceUtilisateur service,
        JwtService jwtService)
    {
        _service = service;
        _jwtService = jwtService;
    }

    // ============================================================
    // GET : Liste paginée des utilisateurs
    // Accessible uniquement à l'administrateur
    // ============================================================

    [Authorize(Policy = "RequireAdmin")]
    [HttpGet]
    public async Task<IActionResult> GetUtilisateurs(
        [FromQuery] PaginationParams paginationParams)
    {
        var resultat =
            await _service.GetUtilisateursPaginesAsync(paginationParams);

        return Ok(resultat);
    }

    // ============================================================
    // GET : Liste simplifiée des contrôleurs
    // Accessible à TOUT utilisateur authentifié (pas seulement l'admin).
    // Utilisée par l'écran "Résultat de Contrôle" pour choisir
    // le ou les contrôleurs d'un lot.
    // ============================================================

    [Authorize]
    [HttpGet("controleurs")]
    public IActionResult GetControleurs()
    {
        var utilisateurs = _service.GetAll();

        var controleurs = utilisateurs
            .Where(u => u.Actif)
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .Select(u => new
            {
                id = u.Id,
                firstName = u.FirstName,
                lastName = u.LastName,
                matricule = u.Matricule
            })
            .ToList();

        return Ok(controleurs);
    }

    // ============================================================
    // GET : Tous les utilisateurs
    // ============================================================

    [Authorize(Policy = "RequireAdmin")]
    [HttpGet("all")]
    public IActionResult GetAll()
    {
        var utilisateurs = _service.GetAll();

        return Ok(utilisateurs);
    }

    // ============================================================
    // GET : Utilisateur par ID
    // Un administrateur peut consulter n'importe quel utilisateur.
    // Un utilisateur normal peut uniquement consulter son propre profil.
    // ============================================================

    [Authorize]
    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                message = "L'identifiant de l'utilisateur est invalide."
            });
        }

        var currentUserId = GetCurrentUserId();

        if (currentUserId == null)
        {
            return Unauthorized(new
            {
                message = "Utilisateur non authentifié."
            });
        }

        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && currentUserId.Value != id)
        {
            return Forbid();
        }

        var user = _service.GetById(id);

        if (user == null)
        {
            return NotFound(new
            {
                message = "Utilisateur introuvable."
            });
        }

        return Ok(user);
    }

    // ============================================================
    // GET : Informations de l'utilisateur connecté
    // ============================================================

    [Authorize]
    [HttpGet("me")]
    public IActionResult GetMe()
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized(new
            {
                message = "Claim id introuvable dans le token JWT."
            });
        }

        var user = _service.GetById(userId.Value);

        if (user == null)
        {
            return NotFound(new
            {
                message = "Utilisateur connecté introuvable."
            });
        }

        return Ok(new
        {
            id = user.Id,
            firstName = user.FirstName,
            lastName = user.LastName,
            email = user.Email,
            matricule = user.Matricule,
            profilId = user.ProfilId,
            estAdmin = user.Profil?.EstAdmin ?? false,
            actif = user.Actif
        });
    }

    // ============================================================
    // POST : Ajouter un utilisateur
    // Administrateur uniquement
    // ============================================================

    [Authorize(Policy = "RequireAdmin")]
    [HttpPost]
    public IActionResult Add([FromBody] Utilisateur user)
    {
        if (user == null)
        {
            return BadRequest(new
            {
                message = "Les données de l'utilisateur sont obligatoires."
            });
        }

        if (string.IsNullOrWhiteSpace(user.FirstName))
        {
            return BadRequest(new
            {
                message = "Le prénom est obligatoire."
            });
        }

        if (string.IsNullOrWhiteSpace(user.LastName))
        {
            return BadRequest(new
            {
                message = "Le nom est obligatoire."
            });
        }

        if (string.IsNullOrWhiteSpace(user.Email))
        {
            return BadRequest(new
            {
                message = "L'adresse email est obligatoire."
            });
        }

        if (string.IsNullOrWhiteSpace(user.Password))
        {
            return BadRequest(new
            {
                message = "Le mot de passe est obligatoire."
            });
        }

        if (string.IsNullOrWhiteSpace(user.Matricule))
        {
            return BadRequest(new
            {
                message = "Le matricule est obligatoire."
            });
        }

        // Nettoyage
        user.FirstName = user.FirstName.Trim();
        user.LastName = user.LastName.Trim();
        user.Email = user.Email.Trim();
        user.Matricule = user.Matricule.Trim();

        // Hash du mot de passe
        user.Password = _service.HashPassword(user.Password);

        // Par défaut, le compte est actif
        user.Actif = true;

        _service.Add(user);
        _service.Commit();

        return Ok(new
        {
            message = "Utilisateur ajouté avec succès.",
            user = new
            {
                id = user.Id,
                firstName = user.FirstName,
                lastName = user.LastName,
                email = user.Email,
                matricule = user.Matricule,
                profilId = user.ProfilId,
                actif = user.Actif
            }
        });
    }

    // ============================================================
    // PUT : Modifier un utilisateur
    // Administrateur uniquement
    // ============================================================

    [Authorize(Policy = "RequireAdmin")]
    [HttpPut("{id:int}")]
    public IActionResult Update(
        int id,
        [FromBody] UserResponseDTO dto)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                message = "L'identifiant de l'utilisateur est invalide."
            });
        }

        if (dto == null)
        {
            return BadRequest(new
            {
                message = "Les données de modification sont obligatoires."
            });
        }

        var user = _service.GetById(id);

        if (user == null)
        {
            return NotFound(new
            {
                message = "Utilisateur introuvable."
            });
        }

        // Mise à jour
        user.FirstName = dto.FirstName?.Trim();
        user.LastName = dto.LastName?.Trim();
        user.Email = dto.Email?.Trim();
        user.Matricule = dto.Matricule?.Trim();
        user.ProfilId = dto.ProfilId;

        _service.Update(user);
        _service.Commit();

        return Ok(new
        {
            message = "Utilisateur modifié avec succès.",
            user
        });
    }

    // ============================================================
    // PUT : Modifier le profil d'un utilisateur
    // Administrateur uniquement
    // ============================================================

    [Authorize(Policy = "RequireAdmin")]
    [HttpPut("{id:int}/profil")]
    public IActionResult ChangeProfil(
        int id,
        [FromBody] int? profilId)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                message = "L'identifiant de l'utilisateur est invalide."
            });
        }

        if (profilId == null)
        {
            return BadRequest(new
            {
                message = "Le profil est obligatoire."
            });
        }

        var user = _service.GetById(id);

        if (user == null)
        {
            return NotFound(new
            {
                message = "Utilisateur introuvable."
            });
        }

        user.ProfilId = profilId;

        _service.Update(user);
        _service.Commit();

        return Ok(new
        {
            message = "Profil modifié avec succès.",
            profilId = user.ProfilId
        });
    }

    // ============================================================
    // DELETE : Supprimer un utilisateur
    // Administrateur uniquement
    // ============================================================

    [Authorize(Policy = "RequireAdmin")]
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                message = "L'identifiant de l'utilisateur est invalide."
            });
        }

        var user = _service.GetById(id);

        if (user == null)
        {
            return NotFound(new
            {
                message = "Utilisateur introuvable."
            });
        }

        _service.DeleteById(id);
        _service.Commit();

        return Ok(new
        {
            message = "Utilisateur supprimé avec succès."
        });
    }

    // ============================================================
    // POST : Signup
    // Inscription publique
    // ============================================================

    [AllowAnonymous]
    [HttpPost("signup")]
    public IActionResult Signup([FromBody] SignupDTO dto)
    {
        if (dto == null)
        {
            return BadRequest(new
            {
                message = "Les données d'inscription sont obligatoires."
            });
        }

        if (string.IsNullOrWhiteSpace(dto.FirstName))
        {
            return BadRequest(new
            {
                message = "Le prénom est obligatoire."
            });
        }

        if (string.IsNullOrWhiteSpace(dto.LastName))
        {
            return BadRequest(new
            {
                message = "Le nom est obligatoire."
            });
        }

        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            return BadRequest(new
            {
                message = "L'adresse email est obligatoire."
            });
        }

        if (string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest(new
            {
                message = "Le mot de passe est obligatoire."
            });
        }

        if (string.IsNullOrWhiteSpace(dto.Matricule))
        {
            return BadRequest(new
            {
                message = "Le matricule est obligatoire."
            });
        }

        var user = new Utilisateur
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = dto.Email.Trim(),
            Password = _service.HashPassword(dto.Password),
            Matricule = dto.Matricule.Trim(),
            Actif = true
        };

        _service.Add(user);
        _service.Commit();

        return Ok(new
        {
            message = "Inscription effectuée avec succès."
        });
    }

    // ============================================================
    // POST : Signin
    // ============================================================

    [AllowAnonymous]
    [HttpPost("signin")]
    public async Task<IActionResult> Signin(
        [FromBody] LoginDTO dto)
    {
        if (dto == null)
        {
            return BadRequest(new
            {
                message = "Les informations de connexion sont obligatoires."
            });
        }

        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            return BadRequest(new
            {
                message = "L'adresse email est obligatoire."
            });
        }

        if (string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest(new
            {
                message = "Le mot de passe est obligatoire."
            });
        }

        var user = await _service.Authenticate(
            dto.Email.Trim(),
            dto.Password
        );

        if (user == null)
        {
            return Unauthorized(new
            {
                message = "Email ou mot de passe incorrect."
            });
        }

        // Vérification du compte
        if (!user.Actif)
        {
            return Unauthorized(new
            {
                message = "Votre compte est désactivé."
            });
        }

        var token = _jwtService.GenerateToken(user);

        return Ok(new
        {
            id = user.Id,
            firstName = user.FirstName,
            lastName = user.LastName,
            email = user.Email,
            matricule = user.Matricule,
            estAdmin = user.Profil?.EstAdmin ?? false,
            profilId = user.ProfilId,
            actif = user.Actif,
            token = token
        });
    }

    // ============================================================
    // PUT : Modifier son propre matricule
    // Utilisateur connecté
    // ============================================================

    [Authorize]
    [HttpPut("me/matricule")]
    public IActionResult UpdateMyMatricule(
        [FromBody] MatriculeDTO dto)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized(new
            {
                message = "Claim id introuvable dans le token JWT."
            });
        }

        if (dto == null ||
            string.IsNullOrWhiteSpace(dto.Matricule))
        {
            return BadRequest(new
            {
                message = "Le matricule est obligatoire."
            });
        }

        var user = _service.GetById(userId.Value);

        if (user == null)
        {
            return NotFound(new
            {
                message = "Utilisateur introuvable."
            });
        }

        user.Matricule = dto.Matricule.Trim();

        _service.Update(user);
        _service.Commit();

        return Ok(new
        {
            message = "Matricule mis à jour avec succès.",
            matricule = user.Matricule
        });
    }

    // ============================================================
    // MÉTHODE PRIVÉE :
    // Récupérer l'ID de l'utilisateur connecté
    // ============================================================

    private int? GetCurrentUserId()
    {
        // Premier choix : claim "id"
        var idClaim = User.FindFirst("id")?.Value;

        // Deuxième possibilité :
        // ClaimTypes.NameIdentifier
        if (string.IsNullOrWhiteSpace(idClaim))
        {
            idClaim = User.FindFirst(
                ClaimTypes.NameIdentifier
            )?.Value;
        }

        if (string.IsNullOrWhiteSpace(idClaim))
        {
            return null;
        }

        if (int.TryParse(idClaim, out int userId))
        {
            return userId;
        }

        return null;
    }
}