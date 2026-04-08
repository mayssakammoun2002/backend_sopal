using Microsoft.AspNetCore.Mvc;
using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.Interfaces;
using Examen.ApplicationCore.DTO;
using System;
using System.IO;

namespace Examen.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TypeDefautController : ControllerBase
    {
        private readonly IServiceTypeDefaut _serviceTypeDefaut;

        public TypeDefautController(IServiceTypeDefaut serviceTypeDefaut)
        {
            _serviceTypeDefaut = serviceTypeDefaut;
        }

        // GET: api/TypeDefaut
        [HttpGet]
        public IActionResult GetAll()
        {
            var types = _serviceTypeDefaut.GetAll();
            return Ok(types);
        }

        // GET: api/TypeDefaut/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var typeDefaut = _serviceTypeDefaut.GetById(id);
            if (typeDefaut == null)
                return NotFound();

            return Ok(typeDefaut);
        }

        // POST: api/TypeDefaut
        [HttpPost]
        [Consumes("multipart/form-data")]
        public IActionResult Add([FromForm] TypeDefautDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(dto.CodeArticle))
                return BadRequest("CodeArticle est obligatoire.");

            string imageName = string.Empty;

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(imagesFolder))
                    Directory.CreateDirectory(imagesFolder);

                imageName = $"{Guid.NewGuid()}{Path.GetExtension(dto.ImageFile.FileName)}";
                var path = Path.Combine(imagesFolder, imageName);

                using var stream = new FileStream(path, FileMode.Create);
                dto.ImageFile.CopyTo(stream);
            }

            var type = new TypeDefaut
            {
                NomDefaut = dto.NomDefaut,
                Description = dto.Description,
                Solution = dto.Solution,
                Frequence = dto.Frequence,
                CodeArticle = dto.CodeArticle,
                ImagePath = imageName
            };

            _serviceTypeDefaut.Add(type);
            _serviceTypeDefaut.Commit();

            return CreatedAtAction(nameof(GetById), new { id = type.Id }, type);
        }

        // PUT: api/TypeDefaut/5
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public IActionResult Update(int id, [FromForm] TypeDefautDto dto)
        {
            var existing = _serviceTypeDefaut.GetById(id);
            if (existing == null)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            existing.NomDefaut = dto.NomDefaut;
            existing.Description = dto.Description;
            existing.Solution = dto.Solution;
            existing.Frequence = dto.Frequence;
            existing.CodeArticle = dto.CodeArticle;

            // Si une nouvelle image est envoyée, on la remplace
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(imagesFolder))
                    Directory.CreateDirectory(imagesFolder);

                var imageName = $"{Guid.NewGuid()}{Path.GetExtension(dto.ImageFile.FileName)}";
                var path = Path.Combine(imagesFolder, imageName);

                using var stream = new FileStream(path, FileMode.Create);
                dto.ImageFile.CopyTo(stream);

                existing.ImagePath = imageName;
            }

            _serviceTypeDefaut.Update(existing);
            _serviceTypeDefaut.Commit();

            return Ok(existing);
        }

        // DELETE: api/TypeDefaut/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var typeDefaut = _serviceTypeDefaut.GetById(id);
            if (typeDefaut == null)
                return NotFound();

            _serviceTypeDefaut.Delete(typeDefaut);
            _serviceTypeDefaut.Commit();

            return NoContent();
        }
    }
}