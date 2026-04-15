using Microsoft.AspNetCore.Mvc;
using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.Interfaces;

namespace Examen.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MachineController : ControllerBase
    {
        private readonly IServiceMachine _serviceMachine;

        public MachineController(IServiceMachine serviceMachine)
        {
            _serviceMachine = serviceMachine;
        }

        [HttpGet]
        public IActionResult GetMachines()
        {
            var machines = _serviceMachine.GetAll();
            return Ok(machines);
        }

        [HttpGet("{code}")]
        public IActionResult GetMachine(string code)
        {
            var machine = _serviceMachine.GetById(code);

            if (machine == null)
                return NotFound();

            return Ok(machine);
        }

        [HttpPost]
        public IActionResult AddMachine([FromBody] Machine machine)
        {
            _serviceMachine.Add(machine);
            _serviceMachine.Commit();

            return CreatedAtAction(nameof(GetMachine),
                new { code = machine.CodeMachine }, machine);
        }

        [HttpPut("{code}")]
        public IActionResult UpdateMachine(string code, Machine machine)
        {
            if (code != machine.CodeMachine)
                return BadRequest();

            _serviceMachine.Update(machine);
            _serviceMachine.Commit();

            return NoContent();
        }

        [HttpDelete("{code}")]
        public IActionResult DeleteMachine(string code)
        {
            var machine = _serviceMachine.GetById(code);

            if (machine == null)
                return NotFound();

            _serviceMachine.Delete(machine);
            _serviceMachine.Commit();

            return NoContent();
        }
    }
}