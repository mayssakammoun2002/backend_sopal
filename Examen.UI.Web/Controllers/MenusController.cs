using System.Collections.Generic;
using System.Linq;
using Examen.ApplicationCore.Entities;
using Examen.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Examen.UI.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenusController : ControllerBase
    {
        private readonly IServiceMenu _serviceMenu;

        public MenusController(IServiceMenu serviceMenu)
        {
            _serviceMenu = serviceMenu;
        }

        // GET: api/menus  (liste plate)
        [HttpGet]
        public ActionResult<List<Menu>> GetAll()
        {
            return Ok(_serviceMenu.GetAll());
        }

        // GET: api/menus/tree  (arborescence : racines seulement, Enfants déjà rempli via include)
        [HttpGet("tree")]
        public ActionResult<List<Menu>> GetTree()
        {
            var all = _serviceMenu.GetAll();
            var racines = all.Where(m => m.ParentId == null).ToList();
            return Ok(racines);
        }

        // GET: api/menus/5
        [HttpGet("{id:int}")]
        public ActionResult<Menu> GetById(int id)
        {
            var menu = _serviceMenu.GetById(id);
            if (menu == null) return NotFound();
            return Ok(menu);
        }

        // POST: api/menus
        [HttpPost]
        public ActionResult<Menu> Create([FromBody] Menu menu)
        {
            var created = _serviceMenu.Create(menu);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/menus/5
        [HttpPut("{id:int}")]
        public ActionResult<Menu> Update(int id, [FromBody] Menu menu)
        {
            var updated = _serviceMenu.Update(id, menu);
            if (updated == null) return NotFound();
            return Ok(updated);
        }
        [HttpGet("mon-menu")]
        [Authorize]
        public ActionResult<List<MenuUtilisateurDto>> GetMonMenu()
        {
            var profilIdClaim = User.FindFirst("profilId")?.Value;
            if (string.IsNullOrEmpty(profilIdClaim) || !int.TryParse(profilIdClaim, out var profilId))
                return Ok(new List<MenuUtilisateurDto>()); // pas de profil = pas de menu

            return Ok(_serviceMenu.GetMenuPourProfil(profilId));
        }
        // DELETE: api/menus/5
        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var deleted = _serviceMenu.Delete(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}