using HMSRoomManagement.Common.Constants;
using HMSRoomManagement.DTO.Room;
using HMSRoomManagement.Service.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RoomManagementService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;
        public RoomsController(IRoomService roomService) => _roomService = roomService;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetRooms([FromQuery] Guid? hotelId, [FromQuery] int? roomTypeId)
            => Ok(await _roomService.GetRoomsAsync(hotelId, roomTypeId));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<RoomDto>> GetRoom(int id)
            => Ok(await _roomService.GetRoomByIdAsync(id));

        [HttpPost]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.TenantAdmin},{Roles.HotelOwner},{Roles.HR}")]
        public async Task<ActionResult<RoomDto>> CreateRoom([FromBody] CreateRoomDto dto)
        {
            var created = await _roomService.CreateRoomAsync(dto);
            return CreatedAtAction(nameof(GetRoom), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.TenantAdmin},{Roles.HotelOwner},{Roles.HR}")]
        public async Task<IActionResult> UpdateRoom(int id, [FromBody] UpdateRoomDto dto)
        {
            await _roomService.UpdateRoomAsync(id, dto);
            return NoContent();
        }

        // Narrow endpoint for the common case: Front Desk / housekeeping just
        // flipping a room's status. Scope + department check lives in the
        // Service layer — see RoomService.UpdateRoomStatusAsync.
        [HttpPatch("{id:int}/status")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.TenantAdmin},{Roles.HotelOwner},{Roles.HR},{Roles.Staff},{Roles.Manager}")]
        public async Task<IActionResult> UpdateRoomStatus(int id, [FromBody] string status)
        {
            await _roomService.UpdateRoomStatusAsync(id, status);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.TenantAdmin},{Roles.HotelOwner}, {Roles.HR}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            await _roomService.DeleteRoomAsync(id);
            return NoContent();
        }
    }
}
