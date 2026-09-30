using HMS.StaffService.DTO.StaffNote;
using HMS.StaffService.Service;
using HMS.StaffService.Service.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.StaffService.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StaffNotesController : ControllerBase
    {
        private readonly IStaffNotesService _noteService;

        public StaffNotesController(IStaffNotesService noteService)
        {
            _noteService = noteService;
        }

        [HttpGet]
        [Authorize(Roles = "HR, Manager")]
        public async Task<ActionResult<IEnumerable<StaffNoteDto>>> GetAll()
        {
            return Ok(await _noteService.GetAllAsync());
        }

        [HttpGet("staff/{staffId:guid}")]
        [Authorize(Roles = "HR, Manager")]
        public async Task<ActionResult<IEnumerable<StaffNoteDto>>> GetByStaffId(Guid staffId)
        {
            return Ok(await _noteService.GetByStaffIdAsync(staffId));
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "HR, Manager")]
        public async Task<ActionResult<StaffNoteDto>> GetById(Guid id)
        {
            return Ok(await _noteService.GetByIdAsync(id));
        }

        [HttpPost]
        [Authorize(Roles = "HR, Manager")]
        public async Task<ActionResult<StaffNoteDto>> Create([FromBody] CreateStaffNoteDto dto)
        {
            var created = await _noteService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.NoteId }, created);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "HR, Manager")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStaffNoteDto dto)
        {
            await _noteService.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _noteService.DeleteAsync(id);
            return NoContent();
        }
    }
}