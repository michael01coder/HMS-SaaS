using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.Service.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace HMS.OperationalService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffTaskAssignmentsController : ControllerBase
    {
        private readonly IStaffTaskAssignmentService _service;

        public StaffTaskAssignmentsController(IStaffTaskAssignmentService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _service.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Assign([FromBody] StaffTaskAssignment assignment)
        {
            var created = await _service.AssignTaskAsync(assignment);
            return CreatedAtAction(nameof(GetById), new { id = created.AssignmentId }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] StaffTaskAssignment assignment)
        {
            if (id != assignment.AssignmentId) return BadRequest("ID mismatch.");
            await _service.UpdateAssignmentAsync(assignment);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
