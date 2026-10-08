using HMSRoomManagement.Common.Constants;
using HMSRoomManagement.DTO.RoomTypeAmenity;
using HMSRoomManagement.Service.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RoomManagementService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RoomTypeAmenitiesController : ControllerBase
    {
        private readonly IRoomTypeAmenityService _service;
        public RoomTypeAmenitiesController(IRoomTypeAmenityService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomTypeAmenityDto>>> GetLinks([FromQuery] int? roomTypeId)
            => Ok(await _service.GetLinksAsync(roomTypeId));

        [HttpPost]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.TenantAdmin},{Roles.HotelOwner},{Roles.Manager}")]
        public async Task<ActionResult<RoomTypeAmenityDto>> CreateLink([FromBody] RoomTypeAmenityDto dto)
        {
            var created = await _service.LinkAsync(dto);
            return CreatedAtAction(nameof(GetLinks), new { roomTypeId = created.RoomTypeId }, created);
        }

        [HttpDelete("{roomTypeId:int}/{amenityId:int}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.TenantAdmin},{Roles.HotelOwner},{Roles.Manager}")]
        public async Task<IActionResult> DeleteLink(int roomTypeId, int amenityId)
        {
            await _service.UnlinkAsync(roomTypeId, amenityId);
            return NoContent();
        }
    }
}
