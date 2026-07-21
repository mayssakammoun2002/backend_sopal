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

    public UtilisateurController(IServiceUtilisateur service, JwtService jwtService)
    {
        _service = service;
        _jwtService = jwtService;
    }

    [Authorize(Policy = "RequireAdmin")]
    [HttpGet]
    public async Task<IActionResult> GetUtilisateurs([FromQuery] PaginationParams paginationParams)
    {
        var resultat = await _service.GetUtilisateursPaginesAsync(paginationParams);
        return Ok(resultat);
    }

    [Authorize(Policy = "RequireAdmin")]
    [HttpGet("all")]
    public IActionResult GetAll() => Ok(_service.GetAll());

    [Authorize]
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var user = _service.GetById(id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult GetMe()
    {
        var idClaim = User.FindFirst("id")?.Value;
        if (idClaim == null)
            return Unauthorized(new { message = "Claim id introuvable" });

        var user = _service.GetById(int.Parse(idClaim));
        if (user == null) return NotFound();

        return Ok(new
        {
            id = user.Id,
            firstName = user.FirstName,
            lastName = user.LastName,
            email = user.Email,
            matricule = user.Matricule,
            profilId = user.ProfilId
        });
    }

    [Authorize(Policy = "RequireAdmin")]
    [HttpPost]
    public IActionResult Add(Utilisateur user)
    {
        user.Password = _service.HashPassword(user.Password);
        _service.Add(user);
        _service.Commit();
        return Ok(user);
    }

    [Authorize(Policy = "RequireAdmin")]
    [HttpPut("{id}")]
    public IActionResult Update(int id, UserResponseDTO dto)
    {
        var user = _service.GetById(id);
        if (user == null) return NotFound();

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.Email = dto.Email;
        user.Matricule = dto.Matricule;
        user.ProfilId = dto.ProfilId;

        _service.Update(user);
        _service.Commit();
        return Ok(user);
    }

    [Authorize(Policy = "RequireAdmin")]
    [HttpPut("{id}/profil")]
    public IActionResult ChangeProfil(int id, [FromBody] int? profilId)
    {
        var user = _service.GetById(id);
        if (user == null) return NotFound();

        user.ProfilId = profilId;
        _service.Update(user);
        _service.Commit();
        return Ok(user);
    }

    [Authorize(Policy = "RequireAdmin")]
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _service.DeleteById(id);
        _service.Commit();
        return NoContent();
    }

    [HttpPost("signup")]
    public IActionResult Signup([FromBody] SignupDTO dto)
    {
        var user = new Utilisateur
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Password = _service.HashPassword(dto.Password),
            Matricule = dto.Matricule,
            Actif = true
        };

        _service.Add(user);
        _service.Commit();
        return Ok();
    }

    [HttpPost("signin")]
    public async Task<IActionResult> Signin([FromBody] LoginDTO dto)
    {
        var user = await _service.Authenticate(dto.Email, dto.Password);
        if (user == null) return Unauthorized();

        var token = _jwtService.GenerateToken(user);
        return Ok(new
        {
            id = user.Id,
            email = user.Email,
            matricule = user.Matricule,
            estAdmin = user.Profil?.EstAdmin ?? false,
            profilId = user.ProfilId,
            token
        });
    }

    // ✅ Nouvel endpoint : l'utilisateur connecté renseigne SON PROPRE matricule
    // (utilisé par le formulaire de contrôle qualité quand le matricule manque)
    [Authorize]
    [HttpPut("me/matricule")]
    public IActionResult UpdateMyMatricule([FromBody] MatriculeDTO dto)
    {
        var idClaim = User.FindFirst("id")?.Value;
        if (idClaim == null)
            return Unauthorized(new { message = "Claim id introuvable" });

        if (string.IsNullOrWhiteSpace(dto?.Matricule))
            return BadRequest(new { message = "Le matricule est obligatoire" });

        var user = _service.GetById(int.Parse(idClaim));
        if (user == null) return NotFound();

        user.Matricule = dto.Matricule.Trim();
        _service.Update(user);
        _service.Commit();

        return Ok(new { matricule = user.Matricule });
    }
}