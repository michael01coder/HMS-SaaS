using HMSHotel.Common.Constants;
using HMSHotel.DTO.HotelAmenity;
using HMSHotel.Service.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMSHotelServices.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HotelAmenitiesController : ControllerBase
    {
        private readonly IHotelAmenityService _service;
        public HotelAmenitiesController(IHotelAmenityService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HotelAmenityDto>>> GetLinks([FromQuery] Guid? hotelId)
        {
            return Ok(await _service.GetLinksAsync(hotelId));
        }

        [HttpPost]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.TenantAdmin},{Roles.HotelOwner},{Roles.HR}")]
        public async Task<ActionResult<HotelAmenityDto>> CreateLink([FromBody] HotelAmenityDto dto)
        {
            var created = await _service.LinkAsync(dto);
            return CreatedAtAction(nameof(GetLinks), new { hotelId = created.HotelId }, created);
        }

        [HttpDelete("{hotelId:guid}/{amenityId:int}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.TenantAdmin},{Roles.HotelOwner},{Roles.HR}")]
        public async Task<IActionResult> DeleteLink(Guid hotelId, int amenityId)
        {
            await _service.UnlinkAsync(hotelId, amenityId);
            return NoContent();
        }
    }
}
