using AuthService.Api.Constraints;
using HMSRoomManagement.Domain.Data;
using HMSRoomManagement.DTO.RoomTypeAmenityDTO;
using HMSRoomManagement.Repository.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RoomManagementService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RoomTypeAmenitiesController : ControllerBase
    {
        private readonly IRoomTypeAmenityRepository _repository;
        public RoomTypeAmenitiesController(IRoomTypeAmenityRepository repository) => _repository = repository;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomTypeAmenityDTO>>> GetLinks([FromQuery] int? roomTypeId)
        {
            var links = roomTypeId.HasValue
                ? await _repository.GetByRoomTypeAsync(roomTypeId.Value)
                : await _repository.GetAllAsync();

            return Ok(links.Select(l => new RoomTypeAmenityDTO { RoomTypeId = l.RoomTypeId, AmenityId = l.AmenityId }));
        }

        [HttpPost]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.HotelOwner},{Roles.HR}")]
        public async Task<ActionResult<RoomTypeAmenityDTO>> CreateLink([FromBody] RoomTypeAmenityDTO dto)
        {
            var existing = await _repository.FindAsync(dto.RoomTypeId, dto.AmenityId);
            if (existing != null) return Conflict(new { message = "This amenity is already linked to this room type." });

            var link = new RoomTypeAmenity { RoomTypeId = dto.RoomTypeId, AmenityId = dto.AmenityId };
            await _repository.AddAsync(link);
            await _repository.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLinks), dto);
        }

        [HttpDelete("{roomTypeId:int}/{amenityId:int}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.HotelOwner},{Roles.HR}")]
        public async Task<IActionResult> DeleteLink(int roomTypeId, int amenityId)
        {
            var existing = await _repository.FindAsync(roomTypeId, amenityId);
            if (existing == null) return NotFound(new { message = "That link doesn't exist." });

            _repository.Remove(existing);
            await _repository.SaveChangesAsync();
            return NoContent();
        }
    }
}
