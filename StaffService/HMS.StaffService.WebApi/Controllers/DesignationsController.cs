using HMS.StaffService.DTO.Department;
using HMS.StaffService.Service.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.StaffService.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DesignationsController : ControllerBase
    {
        private readonly IDesignationService _service;

        public DesignationsController(IDesignationService service) => _service = service;

        [HttpGet]
        [Authorize(Roles = "HR, Manager, Staff, HotelOwner")]
        public async Task<ActionResult<IEnumerable<DesignationDto>>> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id:int}")]
        [Authorize(Roles = "HR, Manager, Staff, HotelOwner")]
        public async Task<ActionResult<DesignationDto>> GetById(int id) => Ok(await _service.GetByIdAsync(id));

        [HttpPost]
        [Authorize(Roles = "HR, HotelOwner")]
        public async Task<ActionResult<DesignationDto>> Create([FromBody] CreateDesignationDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.DesignationId }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "HR, HotelOwner")]
        public async Task<IActionResult> Update(int id, [FromBody] BaseDesignationDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "HR, HotelOwner")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
