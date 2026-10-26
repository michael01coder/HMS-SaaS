using HMS.StaffService.DTO.Staff;
using HMS.StaffService.Service.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.StaffService.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StaffsController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffsController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpGet]
        [Authorize(Roles = "HR, Manager, Staff, HotelOwner")]
        public async Task<ActionResult<IEnumerable<StaffDto>>> GetAll()
        {
            return Ok(await _staffService.GetAllStaffAsync());
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "HR, Manager, Staff, HotelOwner")]
        public async Task<ActionResult<StaffDto>> GetById(Guid id)
        {
            return Ok(await _staffService.GetStaffByIdAsync(id));
        }

        [HttpPost]
        [Authorize(Roles = "HR, HotelOwner")]
        public async Task<ActionResult<StaffDto>> Create([FromBody] CreateStaffDto dto)
        {
            var created = await _staffService.CreateStaffAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.StaffId }, created);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "HR, HotelOwner")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStaffDto dto)
        {
            await _staffService.UpdateStaffAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "HR, HotelOwner")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _staffService.DeleteStaffAsync(id);
            return NoContent();
        }
    }
}