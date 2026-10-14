
using HMS.GuestService.DTO.GuestDTO;
using HMS.GuestService.Service.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace HMS.GuestService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GuestsController : ControllerBase
    {
        private readonly IGuestService _guestService;

        public GuestsController(IGuestService guestService)
        {
            _guestService = guestService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var guests = await _guestService.GetAllGuestsAsync();
            return Ok(guests);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var guest = await _guestService.GetGuestByIdAsync(id);
            return Ok(guest);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGuestDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var guest = await _guestService.CreateGuestAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = guest.GuestId }, guest);
        }

        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGuestDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _guestService.UpdateGuestAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _guestService.DeleteGuestAsync(id);
            return NoContent();
        }
    }
}