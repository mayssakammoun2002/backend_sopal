using Microsoft.AspNetCore.Mvc;
using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.Interfaces;

namespace Examen.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PredictionDefautController : ControllerBase
    {
        private readonly IServicePredictionDefaut _servicePrediction;

        public PredictionDefautController(IServicePredictionDefaut servicePrediction)
        {
            _servicePrediction = servicePrediction;
        }

        // GET api/predictiondefaut
        [HttpGet]
        public IActionResult GetPredictions()
        {
            var predictions = _servicePrediction.GetAll();
            return Ok(predictions);
        }

        // GET api/predictiondefaut/5
        [HttpGet("{id:int}")]
        public IActionResult GetPrediction(int id)
        {
            var prediction = _servicePrediction.GetById(id);
            if (prediction == null)
                return NotFound();
            return Ok(prediction);
        }

        // GET api/predictiondefaut/bycontrole/RC-001
        [HttpGet("bycontrole/{resultatControleId}")]
        public IActionResult GetByResultatControle(string resultatControleId)
        {
            var predictions = _servicePrediction.GetByResultatControle(resultatControleId);
            return Ok(predictions);
        }

        // POST api/predictiondefaut
        [HttpPost]
        public IActionResult AddPrediction([FromBody] PredictionDefaut predictionDefaut)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            predictionDefaut.DatePrediction = DateTime.UtcNow;

            _servicePrediction.Add(predictionDefaut);
            _servicePrediction.Commit();

            return CreatedAtAction(nameof(GetPrediction),
                new { id = predictionDefaut.Id }, predictionDefaut);
        }

        // PUT api/predictiondefaut/5
        [HttpPut("{id:int}")]
        public IActionResult UpdatePrediction(int id, [FromBody] PredictionDefaut predictionDefaut)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = _servicePrediction.GetById(id);
            if (existing == null)
                return NotFound();

            existing.EstDefectueux = predictionDefaut.EstDefectueux;
            existing.Probabilite = predictionDefaut.Probabilite;
            existing.NiveauRisque = predictionDefaut.NiveauRisque;
            existing.TypeDefautPreditId = predictionDefaut.TypeDefautPreditId;
            existing.ResultatControleId = predictionDefaut.ResultatControleId;

            _servicePrediction.Update(existing);
            _servicePrediction.Commit();

            return NoContent();
        }
        [HttpPost("predict")]
        public async Task<IActionResult> Predict([FromBody] object data)
        {
            var result = await _servicePrediction.PredireAsync(data);

            return Ok(result);
        }
        // DELETE api/predictiondefaut/5
        [HttpDelete("{id:int}")]
        public IActionResult DeletePrediction(int id)
        {
            var existing = _servicePrediction.GetById(id);
            if (existing == null)
                return NotFound();

            _servicePrediction.Delete(existing);
            _servicePrediction.Commit();

            return NoContent();
        }
    }
}