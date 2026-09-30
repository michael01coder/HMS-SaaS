using AuthService.Api.Constraints;
using HMSRoomManagement.Domain.Data;
using HMSRoomManagement.DTO.RoomTypeDTO;
using HMSRoomManagement.Repository.Contract;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace RoomManagementService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RoomTypesController : ControllerBase
    {
        private readonly IRoomTypeRepository _roomTypeRepository;

        public RoomTypesController(IRoomTypeRepository roomTypeRepository)
        {
            _roomTypeRepository = roomTypeRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomTypeDTO>>> GetRoomTypes([FromQuery] Guid? hotelId)
        {
            var roomTypes = hotelId.HasValue
                ? await _roomTypeRepository.FindAsync(rt => rt.HotelId == hotelId.Value)
                : await _roomTypeRepository.GetAllAsync();

            return Ok(roomTypes.Adapt<IEnumerable<RoomTypeDTO>>());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<RoomTypeDTO>> GetRoomType(int id)
        {
            var roomType = await _roomTypeRepository.GetByIdAsync(id);
            if (roomType == null)
                return NotFound(new { message = $"Room type {id} not found." });

            return Ok(roomType.Adapt<RoomTypeDTO>());
        }

        [HttpPost]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.HotelOwner},{Roles.HR}")]
        public async Task<ActionResult<RoomTypeDTO>> CreateRoomType([FromBody] CreateRoomTypeDTO dto)
        {
            var roomType = dto.Adapt<RoomType>();
            await _roomTypeRepository.AddAsync(roomType);
            await _roomTypeRepository.SaveChangesAsync();

            var result = roomType.Adapt<RoomTypeDTO>();
            return CreatedAtAction(nameof(GetRoomType), new { id = roomType.Id }, result);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.HotelOwner},{Roles.HR}")]
        public async Task<ActionResult<RoomTypeDTO>> UpdateRoomType(int id, [FromBody] UpdateRoomTypeDTO dto)
        {
            var existing = await _roomTypeRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Room type {id} not found." });

            dto.Adapt(existing);
            _roomTypeRepository.Update(existing);
            await _roomTypeRepository.SaveChangesAsync();

            return Ok(existing.Adapt<RoomTypeDTO>());
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.HotelOwner}")]
        public async Task<IActionResult> DeleteRoomType(int id)
        {
            var existing = await _roomTypeRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Room type {id} not found." });

            _roomTypeRepository.Delete(existing);
            await _roomTypeRepository.SaveChangesAsync();
            return NoContent();
        }
    }
}