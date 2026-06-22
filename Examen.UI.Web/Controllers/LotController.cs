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

        [HttpGet]
        public IActionResult GetAll()
        {
            var lots = _serviceLot.GetAll();
            return Ok(lots);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var lot = _serviceLot.GetById(id);
            if (lot == null) return NotFound();
            return Ok(lot);
        }

        [HttpGet("machine/{machineId}")]
        public IActionResult GetByMachine(string machineId)
        {
            return Ok(_serviceLot.GetByMachine(machineId));
        }

        [HttpGet("statut/{statut}")]
        public IActionResult GetByStatut(string statut)
        {
            return Ok(_serviceLot.GetByStatut(statut));
        }

        // ✅ FIX : reçoit CreateLotDto, pas Lot
        [HttpPost]
        public IActionResult Create([FromBody] CreateLotDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var lot = new Lot
            {
                NumeroLot = dto.NumeroLot,
                MachineId = dto.MachineId,        // string FK
                ProduitId = dto.ProduitId,        // string FK ✅
                OperateurId = dto.OperateurId,      // int FK
                DateDebut = dto.DateDebut,
                DateFin = dto.DateFin,
                QuantitePrevue = dto.QuantitePrevue,
                QuantiteProduite = dto.QuantiteProduite,
                Statut = dto.Statut,
                Commentaire = dto.Commentaire
            };

            _serviceLot.Add(lot);
            _serviceLot.Commit();
            return CreatedAtAction(nameof(GetById), new { id = lot.Id }, lot);
        }

        // ✅ FIX : reçoit CreateLotDto, pas Lot
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] CreateLotDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = _serviceLot.GetById(id);
            if (existing == null) return NotFound();

            existing.NumeroLot = dto.NumeroLot;
            existing.MachineId = dto.MachineId;
            existing.ProduitId = dto.ProduitId;
            existing.OperateurId = dto.OperateurId;
            existing.DateDebut = dto.DateDebut;
            existing.DateFin = dto.DateFin;
            existing.QuantitePrevue = dto.QuantitePrevue;
            existing.QuantiteProduite = dto.QuantiteProduite;
            existing.Statut = dto.Statut;
            existing.Commentaire = dto.Commentaire;
            existing.UpdatedAt = DateTime.UtcNow;

            _serviceLot.Update(existing);
            _serviceLot.Commit();
            return NoContent();
        }

        [HttpPatch("{id:int}/statut")]
        public IActionResult ChangeStatut(int id, [FromBody] string statut)
        {
            var existing = _serviceLot.GetById(id);
            if (existing == null) return NotFound();

            if (!StatutLot.IsValid(statut))
                return BadRequest($"Statut inconnu : {statut}");

            existing.Statut = statut;
            existing.UpdatedAt = DateTime.UtcNow;
            _serviceLot.Update(existing);
            _serviceLot.Commit();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var existing = _serviceLot.GetById(id);
            if (existing == null) return NotFound();

            _serviceLot.Delete(existing);
            _serviceLot.Commit();
            return NoContent();
        }
    }
}