using Examen.ApplicationCore.Domain;
using Examen.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Examen.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SeuilsController : ControllerBase
    {
        private readonly ExamenDbContext _db;

        public SeuilsController(ExamenDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? codeMachine)
        {
            var query = _db.Seuils.Include(s => s.Machine)
                                  .Include(s => s.Produit)
                                  .AsQueryable();

            if (!string.IsNullOrEmpty(codeMachine))
                query = query.Where(s => s.CodeMachine == codeMachine);

            var seuils = await query.OrderBy(s => s.CodeMachine).ToListAsync();
            return Ok(seuils);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var seuil = await _db.Seuils
                .Include(s => s.Machine)
                .Include(s => s.Produit)
                .FirstOrDefaultAsync(s => s.Id == id);

            return seuil is null ? NotFound() : Ok(seuil);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Seuil seuil)
        {
            seuil.DateCreation = DateTime.UtcNow;
            seuil.CreePar = UserId(); // À améliorer avec JWT

            _db.Seuils.Add(seuil);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = seuil.Id }, seuil);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Seuil seuil)
        {
            if (id != seuil.Id) return BadRequest();

            seuil.DateModification = DateTime.UtcNow;

            _db.Entry(seuil).State = EntityState.Modified;
            _db.Entry(seuil).Property(x => x.DateCreation).IsModified = false;
            _db.Entry(seuil).Property(x => x.CreePar).IsModified = false;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var seuil = await _db.Seuils.FindAsync(id);
            if (seuil == null) return NotFound();

            seuil.EstActif = false; // Soft delete
            await _db.SaveChangesAsync();
            return NoContent();
        }

        private int UserId()
        {
            var idClaim = User.FindFirst("id")?.Value;
            return int.TryParse(idClaim, out int id) ? id : 1;
        }
    }
}