using HMSHotel.Common.Constants;
using HMSHotel.DTO.Amenity;
using HMSHotel.Service.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMSHotelServices.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AmenitiesController : ControllerBase
    {
        private readonly IAmenityService _amenityService;
        public AmenitiesController(IAmenityService amenityService) => _amenityService = amenityService;

        // GET: api/Amenities — master list, readable by any authenticated role.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AmenityDto>>> GetAmenities([FromQuery] string? category)
        {
            return Ok(await _amenityService.GetAmenitiesAsync(category));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AmenityDto>> GetAmenity(int id)
        {
            return Ok(await _amenityService.GetAmenityByIdAsync(id));
        }

        // Amenities are platform/tenant-level master data.
        [HttpPost]
        [Authorize(Roles = $"{Roles.HR},{Roles.TenantAdmin}")]
        public async Task<ActionResult<AmenityDto>> CreateAmenity([FromBody] CreateAmenityDto dto)
        {
            var created = await _amenityService.CreateAmenityAsync(dto);
            return CreatedAtAction(nameof(GetAmenity), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = $"{Roles.HR},{Roles.TenantAdmin}")]
        public async Task<IActionResult> UpdateAmenity(int id, [FromBody] UpdateAmenityDto dto)
        {
            await _amenityService.UpdateAmenityAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = $"{Roles.HR},{Roles.TenantAdmin}")]
        public async Task<IActionResult> DeleteAmenity(int id)
        {
            await _amenityService.DeleteAmenityAsync(id);
            return NoContent();
        }
    }
}
