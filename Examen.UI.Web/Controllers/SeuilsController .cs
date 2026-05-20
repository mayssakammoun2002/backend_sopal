using System;
using System.Threading.Tasks;
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

        // GET api/seuils
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? machineId)
        {
            var query = _db.Seuils.AsQueryable();
            if (!string.IsNullOrEmpty(machineId))
                query = query.Where(s => s.CodeMachine == machineId);
            return Ok(await query.OrderBy(s => s.CodeMachine).ToListAsync());
        }

        // GET api/seuils/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var seuil = await _db.Seuils.FindAsync(id);
            return seuil is null ? NotFound() : Ok(seuil);
        }

        // POST api/seuils
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Seuil seuil)
        {
            seuil.DateCreation = DateTime.UtcNow;
            seuil.CreePar = 1; // TODO: userId depuis JWT
            _db.Seuils.Add(seuil);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = seuil.Id }, seuil);
        }

        // PUT api/seuils/5
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

        // DELETE api/seuils/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var seuil = await _db.Seuils.FindAsync(id);
            if (seuil is null) return NotFound();
            seuil.EstActif = false; // Soft delete
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}