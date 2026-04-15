using System.Security.Claims;
using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.DTOs;
using Examen.ApplicationCore.Interfaces;
using Examen.ApplicationCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

    // 🔐 ADMIN ONLY
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult GetAll() => Ok(_service.GetAll());

    [Authorize]
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var user = _service.GetById(id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult Add(Utilisateur user)
    {
        user.Password = _service.HashPassword(user.Password);
        _service.Add(user);
        _service.Commit();
        return Ok(user);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public IActionResult Update(int id, UserResponseDTO dto)
    {
        var user = _service.GetById(id);
        if (user == null) return NotFound();

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.Email = dto.Email;
        user.Role = (Role)dto.Role;

        _service.Update(user);
        _service.Commit();

        return Ok(user);
    }
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/role")]
    public IActionResult ChangeRole(int id, [FromBody] Role role)
    {
        var user = _service.GetById(id);
        if (user == null) return NotFound();

        user.Role = role;
        _service.Update(user);
        _service.Commit();

        return Ok(user);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _service.DeleteById(id);
        _service.Commit();
        return NoContent();
    }

    // 🔓 AUTH
    [HttpPost("signup")]
    public IActionResult Signup([FromBody] SignupDTO dto)
    {
        var user = new Utilisateur
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Password = _service.HashPassword(dto.Password),
            Role = Role.User, // 🔥 TOUJOURS USER
            Actif = true
        };

        _service.Add(user);
        _service.Commit();
        return Ok();
    }
    [HttpPost("signin")]
    public IActionResult Signin([FromBody] LoginDTO dto)
    {
        var user = _service.GetByEmail(dto.Email);

        if (user == null || !_service.VerifyPassword(user, dto.Password))
            return Unauthorized("Email ou mot de passe incorrect");

        var token = _jwtService.GenerateToken(user);

        return Ok(new
        {
            id = user.Id,
            email = user.Email,
            role = user.Role.ToString(), // ✅ minuscule
            token = token // ✅ minuscule
        });
    }

    [Authorize]
[HttpGet("me")]
public IActionResult GetCurrentUser()
{
    // 🔹 récupérer email depuis token
    var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

    if (email == null)
        return Unauthorized();

    var user = _service.GetByEmail(email);

    if (user == null)
        return NotFound();

    return Ok(new
    {
        user.Id,
        user.FirstName,
        user.LastName,
        user.Email,
        Role = user.Role.ToString()
    });
}


}