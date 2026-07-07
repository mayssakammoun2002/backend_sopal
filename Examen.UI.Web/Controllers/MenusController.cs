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

        [Authorize(Policy = "RequireAdmin")]
        [HttpGet]
        public ActionResult<List<Menu>> GetAll()
        {
            return Ok(_serviceMenu.GetAll());
        }

        [Authorize(Policy = "RequireAdmin")]
        [HttpGet("tree")]
        public ActionResult<List<Menu>> GetTree()
        {
            var all = _serviceMenu.GetAll();
            var racines = all.Where(m => m.ParentId == null).ToList();
            return Ok(racines);
        }

        [Authorize(Policy = "RequireAdmin")]
        [HttpGet("{id:int}")]
        public ActionResult<Menu> GetById(int id)
        {
            var menu = _serviceMenu.GetById(id);
            if (menu == null) return NotFound();
            return Ok(menu);
        }

        [Authorize(Policy = "RequireAdmin")]
        [HttpPost]
        public ActionResult<Menu> Create([FromBody] Menu menu)
        {
            var created = _serviceMenu.Create(menu);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [Authorize(Policy = "RequireAdmin")]
        [HttpPut("{id:int}")]
        public ActionResult<Menu> Update(int id, [FromBody] Menu menu)
        {
            var updated = _serviceMenu.Update(id, menu);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [Authorize(Policy = "RequireAdmin")]
        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var deleted = _serviceMenu.Delete(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        // ✅ Accessible à TOUT utilisateur connecté (pas seulement admin) — chacun récupère son propre menu
        [Authorize]
        [HttpGet("mon-menu")]
        public ActionResult<List<MenuUtilisateurDto>> GetMonMenu()
        {
            var estAdmin = User.FindFirst("estAdmin")?.Value == "true";

            if (estAdmin)
                return Ok(_serviceMenu.GetMenuPourAdmin());

            var profilIdClaim = User.FindFirst("profilId")?.Value;
            if (string.IsNullOrEmpty(profilIdClaim) || !int.TryParse(profilIdClaim, out var profilId))
                return Ok(new List<MenuUtilisateurDto>());

            return Ok(_serviceMenu.GetMenuPourProfil(profilId));
        }
    }
}