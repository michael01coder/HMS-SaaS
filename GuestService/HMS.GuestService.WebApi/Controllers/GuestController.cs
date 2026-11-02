using HMS.GuestService.DTO.GuestDTO;
using HMS.GuestService.Service.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.GuestService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "HotelOwner, HR, Manager, Staff")]
    public class GuestsController : ControllerBase
    {
        private readonly IGuestService _guestService;

        public GuestsController(IGuestService guestService) => _guestService = guestService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _guestService.GetAllGuestsAsync());

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id) => Ok(await _guestService.GetGuestByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGuestDTO dto)
        {
            var guest = await _guestService.CreateGuestAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = guest.GuestId }, guest);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGuestDTO dto)
        {
            await _guestService.UpdateGuestAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _guestService.DeleteGuestAsync(id);
            return NoContent();
        }
    }
}
