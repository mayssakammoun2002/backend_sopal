using Microsoft.AspNetCore.Mvc;
using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.Interfaces;
using Examen.ApplicationCore.DTOs;
using Microsoft.AspNetCore.Authorization;
using Examen.ApplicationCore.Services;

namespace Examen.Web.Controllers
{
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

        // ---------------- CRUD classique ----------------
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetAll() => Ok(_service.GetAll());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _service.GetById(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public IActionResult Add(Utilisateur user)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _service.Add(user);
            _service.Commit();
            return Ok(user);
        }

        [HttpPut("{id}/role")]
        public IActionResult ChangeRole(int id, [FromBody] Role role)
        {
            var user = _service.GetById(id);
            if (user == null) return NotFound();

            user.Role = role;
            _service.Update(user);
            _service.Commit();

            return Ok(new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                Role = user.Role.ToString()
            });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Utilisateur updatedUser)
        {
            var user = _service.GetById(id);
            if (user == null) return NotFound();

            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email;

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

        // ---------------- Auth ----------------
        [HttpPost("signup")]
        public IActionResult Signup([FromBody] SignupDTO dto)
        {
            var user = new Utilisateur
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = _service.HashPassword(dto.Password),
                Role = Role.User,
                Actif = true
            };

            _service.Add(user);
            _service.Commit();
            return Ok();
        }

        [HttpPost("signin")]
        public IActionResult Signin([FromBody] LoginDTO dto)
        {
            try
            {
                var user = _service.GetByEmail(dto.Email);
                if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                    return Unauthorized("Email ou mot de passe incorrect");

                var token = _jwtService.GenerateToken(user);
                return Ok(new
                {
                    user.Id,
                    user.Email,
                    user.Role,
                    Token = token
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stackTrace = ex.StackTrace });
            }
        }
    }
}