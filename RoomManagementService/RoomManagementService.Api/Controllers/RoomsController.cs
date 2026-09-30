using AuthService.Api.Constraints;
using HMSRoomManagement.Domain.Data;
using HMSRoomManagement.DTO.RoomDTO;
using HMSRoomManagement.Repository.Contract;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RoomManagementService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomRepository _roomRepository;

        public RoomsController(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomDTO>>> GetRooms([FromQuery] Guid? hotelId, [FromQuery] int? roomTypeId)
        {
            var rooms = await _roomRepository.FindAsync(r =>
                (!hotelId.HasValue || r.HotelId == hotelId.Value) &&
                (!roomTypeId.HasValue || r.RoomTypeId == roomTypeId.Value));

            return Ok(rooms.Adapt<IEnumerable<RoomDTO>>());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<RoomDTO>> GetRoom(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null)
                return NotFound(new { message = $"Room {id} not found." });

            return Ok(room.Adapt<RoomDTO>());
        }

        [HttpPost]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.HotelOwner},{Roles.HR}")]
        public async Task<ActionResult<RoomDTO>> CreateRoom([FromBody] CreateRoomDTO dto)
        {
            var room = dto.Adapt<Room>();
            await _roomRepository.AddAsync(room);
            await _roomRepository.SaveChangesAsync();

            var result = room.Adapt<RoomDTO>();
            return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, result);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.HotelOwner},{Roles.HR}")]
        public async Task<ActionResult<RoomDTO>> UpdateRoom(int id, [FromBody] UpdateRoomDTO dto)
        {
            var existing = await _roomRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Room {id} not found." });

            dto.Adapt(existing);
            _roomRepository.Update(existing);
            await _roomRepository.SaveChangesAsync();

            return Ok(existing.Adapt<RoomDTO>());
        }

     
        [HttpPatch("{id:int}/status")]
        [Authorize(Roles = $"{Roles.HR},{Roles.FrontDeskStaff}")]
        public async Task<ActionResult<RoomDTO>> UpdateRoomStatus(int id, [FromBody] string status)
        {
            var existing = await _roomRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Room {id} not found." });

            existing.Status = status;
            _roomRepository.Update(existing);
            await _roomRepository.SaveChangesAsync();

            return Ok(existing.Adapt<RoomDTO>());
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.HotelOwner}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var existing = await _roomRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Room {id} not found." });

            _roomRepository.Delete(existing);
            await _roomRepository.SaveChangesAsync();
            return NoContent();
        }
    }
}