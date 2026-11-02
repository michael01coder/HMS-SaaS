using HMS.OperationalService.DTO;
using HMS.OperationalService.Service.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.OperationalService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "HotelOwner, HR, Manager, Staff")]
    public class LostAndFoundController : ControllerBase
    {
        private readonly ILostAndFoundService _service;

        public LostAndFoundController(ILostAndFoundService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _service.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LostAndFoundDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.LostAndFoundId }, created);
        }

        [HttpPut("{id:int}/claim")]
        public async Task<IActionResult> Claim(int id, [FromQuery] Guid guestId)
        {
            await _service.ClaimItemAsync(id, guestId);
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
