using HMSHotel.Common.Constants;
using HMSHotel.DTO.Hotel;
using HMSHotel.Service.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMSHotelServices.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelService _hotelService;

        public HotelsController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        // GET: api/Hotels — scoped by role inside the Service layer
        // (SuperAdmin: all, TenantAdmin: own tenant, everyone else: own hotel).
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HotelDto>>> GetHotels()
        {
            return Ok(await _hotelService.GetAllHotelsAsync());
        }

        // GET: api/Hotels/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<HotelDto>> GetHotel(Guid id)
        {
            return Ok(await _hotelService.GetHotelByIdAsync(id));
        }

        // POST: api/Hotels — onboarding a new property under a tenant.
        [HttpPost]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.TenantAdmin}")]
        public async Task<ActionResult<HotelDto>> CreateHotel([FromBody] CreateHotelDto dto)
        {
            var created = await _hotelService.CreateHotelAsync(dto);
            return CreatedAtAction(nameof(GetHotel), new { id = created.Id }, created);
        }

        // PUT: api/Hotels/{id} — SuperAdmin/TenantAdmin/HotelOwner, scoped in Service layer.
        [HttpPut("{id:guid}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.TenantAdmin},{Roles.HotelOwner}")]
        public async Task<IActionResult> UpdateHotel(Guid id, [FromBody] UpdateHotelDto dto)
        {
            await _hotelService.UpdateHotelAsync(id, dto);
            return NoContent();
        }

        // DELETE: api/Hotels/{id}
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.TenantAdmin}")]
        public async Task<IActionResult> DeleteHotel(Guid id)
        {
            await _hotelService.DeleteHotelAsync(id);
            return NoContent();
        }
    }
}
