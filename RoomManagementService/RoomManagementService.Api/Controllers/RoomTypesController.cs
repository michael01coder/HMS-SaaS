using HMSRoomManagement.Common.Constants;
using HMSRoomManagement.DTO.RoomType;
using HMSRoomManagement.Service.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RoomManagementService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RoomTypesController : ControllerBase
    {
        private readonly IRoomTypeService _roomTypeService;
        public RoomTypesController(IRoomTypeService roomTypeService) => _roomTypeService = roomTypeService;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomTypeDto>>> GetRoomTypes([FromQuery] Guid? hotelId)
            => Ok(await _roomTypeService.GetRoomTypesAsync(hotelId));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<RoomTypeDto>> GetRoomType(int id)
            => Ok(await _roomTypeService.GetRoomTypeByIdAsync(id));

        [HttpPost]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.TenantAdmin},{Roles.HotelOwner},{Roles.HR}")]
        public async Task<ActionResult<RoomTypeDto>> CreateRoomType([FromBody] CreateRoomTypeDto dto)
        {
            var created = await _roomTypeService.CreateRoomTypeAsync(dto);
            return CreatedAtAction(nameof(GetRoomType), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.TenantAdmin},{Roles.HotelOwner},{Roles.HR}")]
        public async Task<IActionResult> UpdateRoomType(int id, [FromBody] UpdateRoomTypeDto dto)
        {
            await _roomTypeService.UpdateRoomTypeAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.TenantAdmin},{Roles.HotelOwner},{Roles.HR}")]
        public async Task<IActionResult> DeleteRoomType(int id)
        {
            await _roomTypeService.DeleteRoomTypeAsync(id);
            return NoContent();
        }
    }
}
