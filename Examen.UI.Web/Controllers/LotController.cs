using Microsoft.AspNetCore.Mvc;
using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.Interfaces;

namespace Examen.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LotController : ControllerBase
    {
        private readonly IServiceLot _serviceLot;

        public LotController(IServiceLot serviceLot)
        {
            _serviceLot = serviceLot;
        }

        // GET api/lot
        [HttpGet]
        public IActionResult GetAll()
        {
            var lots = _serviceLot.GetAll();
            return Ok(lots);
        }

        // GET api/lot/5
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var lot = _serviceLot.GetById(id);
            if (lot == null)
                return NotFound();
            return Ok(lot);
        }

        // GET api/lot/machine/M001
        [HttpGet("machine/{machineId}")]
        public IActionResult GetByMachine(string machineId)
        {
            var lots = _serviceLot.GetByMachine(machineId);
            return Ok(lots);
        }

        // GET api/lot/statut/En cours
        [HttpGet("statut/{statut}")]
        public IActionResult GetByStatut(string statut)
        {
            var lots = _serviceLot.GetByStatut(statut);
            return Ok(lots);
        }

        // POST api/lot
        [HttpPost]
        public IActionResult Create([FromBody] Lot lot)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _serviceLot.Add(lot);
            _serviceLot.Commit();

            return CreatedAtAction(nameof(GetById), new { id = lot.Id }, lot);
        }

        // PUT api/lot/5
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] Lot lot)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = _serviceLot.GetById(id);
            if (existing == null)
                return NotFound();

            // Mise à jour des champs métier uniquement (jamais l'Id ni les FK critiques)
            existing.NumeroLot = lot.NumeroLot;
            existing.MachineId = lot.MachineId;
            existing.ProduitId = lot.ProduitId;
            existing.OperateurId = lot.OperateurId;
            existing.DateDebut = lot.DateDebut;
            existing.DateFin = lot.DateFin;
            existing.QuantitePrevue = lot.QuantitePrevue;
            existing.QuantiteProduite = lot.QuantiteProduite;
            existing.Statut = lot.Statut;
            existing.Commentaire = lot.Commentaire;
            existing.UpdatedAt = DateTime.UtcNow;

            _serviceLot.Update(existing);
            _serviceLot.Commit();

            return NoContent();
        }

        // PATCH api/lot/5/statut  — changer le statut seul (ex: suspendre, terminer)
        [HttpPatch("{id:int}/statut")]
        public IActionResult ChangeStatut(int id, [FromBody] string statut)
        {
            var existing = _serviceLot.GetById(id);
            if (existing == null)
                return NotFound();

            if (!StatutLot.IsValid(statut))
                return BadRequest($"Statut inconnu : {statut}");

            existing.Statut = statut;
            existing.UpdatedAt = DateTime.UtcNow;

            _serviceLot.Update(existing);
            _serviceLot.Commit();

            return NoContent();
        }

        // DELETE api/lot/5
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var existing = _serviceLot.GetById(id);
            if (existing == null)
                return NotFound();

            _serviceLot.Delete(existing);
            _serviceLot.Commit();

            return NoContent();
        }
    }
}