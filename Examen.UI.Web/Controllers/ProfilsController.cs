using System;
using System.Collections.Generic;
using Examen.ApplicationCore.Interfaces;
using Examen.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Examen.UI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfilsController : ControllerBase
    {
        private readonly IServiceProfil _serviceProfil;

        public ProfilsController(IServiceProfil serviceProfil)
        {
            _serviceProfil = serviceProfil;
        }

        // GET: api/profils
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(_serviceProfil.GetAll());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, detail = ex.InnerException?.Message });
            }
        }

        // GET: api/profils/5
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var profil = _serviceProfil.GetById(id);
                if (profil == null)
                    return NotFound(new { message = $"Profil avec l'id {id} introuvable." });

                return Ok(profil);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, detail = ex.InnerException?.Message });
            }
        }

        // POST: api/profils
        [HttpPost]
        public IActionResult Create([FromBody] CreateProfilRequest request)
        {
            try
            {
                if (request == null || !ModelState.IsValid)
                    return BadRequest(ModelState);

                var created = _serviceProfil.Create(request);

                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, detail = ex.InnerException?.Message });
            }
        }

        // PUT: api/profils/5
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] CreateProfilRequest request)
        {
            try
            {
                if (request == null || !ModelState.IsValid)
                    return BadRequest(ModelState);

                var updated = _serviceProfil.Update(id, request);
                if (updated == null)
                    return NotFound(new { message = $"Profil avec l'id {id} introuvable." });

                return Ok(updated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, detail = ex.InnerException?.Message });
            }
        }

        // DELETE: api/profils/5
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var deleted = _serviceProfil.Delete(id);
                if (!deleted)
                    return NotFound(new { message = $"Profil avec l'id {id} introuvable." });

                return Ok(new { success = true, message = "Profil supprimé avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, detail = ex.InnerException?.Message });
            }
        }

        // PUT: api/profils/5/menus
        [HttpPut("{id:int}/menus")]
        public IActionResult UpdateMenus(int id, [FromBody] List<ProfilMenuRequest> menus)
        {
            try
            {
                if (menus == null)
                    return BadRequest(new { message = "La liste des menus est requise." });

                var ok = _serviceProfil.UpdateMenus(id, menus);
                if (!ok)
                    return NotFound(new { message = $"Profil avec l'id {id} introuvable." });

                return Ok(new { success = true, message = "Menus mis à jour avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, detail = ex.InnerException?.Message });
            }
        }

        // PUT: api/profils/5/droits
        [HttpPut("{id:int}/droits")]
        public IActionResult UpdateDroits(int id, [FromBody] List<ProfilFonctionDroitRequest> droits)
        {
            try
            {
                if (droits == null)
                    return BadRequest(new { message = "La liste des droits est requise." });

                var ok = _serviceProfil.UpdateDroits(id, droits);
                if (!ok)
                    return NotFound(new { message = $"Profil avec l'id {id} introuvable." });

                return Ok(new { success = true, message = "Droits mis à jour avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, detail = ex.InnerException?.Message });
            }
        }
    }
}   