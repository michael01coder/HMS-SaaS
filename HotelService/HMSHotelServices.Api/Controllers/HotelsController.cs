
using AuthService.Api.Constraints;
using HMSHotel.Domain.Data;
using HMSHotel.DTO.HotelDTO;
using HMSHotel.Repository.Contract;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelRepository _hotelRepository;
        //Injected IHotelRepository via Constructor Depedency Injection Here
        public HotelsController(IHotelRepository hotelRepository)
        {
            _hotelRepository = hotelRepository;
        }
        //Get All Hotels 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HotelDTO>>> GetHotels()
        {
            var hotels = await _hotelRepository.GetAllAsync();
            return Ok(hotels.Adapt<IEnumerable<HotelDTO>>());
        }
        //Get Hotel By ID
        [HttpGet("{HotelId:guid}")]
        public async Task<ActionResult<HotelDTO>> GetHotel(Guid HotelId)
        {
            var hotel = await _hotelRepository.GetByIdAsync(HotelId);

            if (hotel == null)
            {
                return NotFound(new { message = $"Hotel with {HotelId} is not found!" });
            }
            return Ok(hotel.Adapt<HotelDTO>());
        }
        //Create A Hotel in DB
        [HttpPost]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.TenantAdmin}")]
        public async Task<ActionResult<HotelDTO>> CreateHotel([FromBody] CreateHotelDTO dto)
        {
            try
            {
                var hotel = dto.Adapt<Hotel>();
                if (hotel.Id == Guid.Empty)
                {
                    hotel.Id = Guid.NewGuid();
                }
                await _hotelRepository.AddAsync(hotel);
                await _hotelRepository.SaveChangesAsync();

                var resultDto = hotel.Adapt<HotelDTO>();
                return CreatedAtAction(nameof(GetHotel), new { HotelId = hotel.Id }, resultDto);
            }
            catch (Exception ex)
            {
                // Returns exact internal error details to help debug missing fields or FK errors
                return StatusCode(500, new
                {
                    message = "Failed to save hotel to PostgreSQL.",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        //Update the Existing Hotel in DB
        [HttpPut("{HotelId:guid}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.TenantAdmin},{Roles.HR}")]
        public async Task<ActionResult<HotelDTO>> UpdateHotel(Guid HotelId, [FromBody] UpdateHotelDTO dto)
        {
            var existinghotel = await _hotelRepository.GetByIdAsync(HotelId);
            if (existinghotel == null)
            {
                return NotFound(new { message = $"Hotel with ID:{HotelId} not found " });
            }
            dto.Adapt(existinghotel);

            _hotelRepository.Update(existinghotel);
            await _hotelRepository.SaveChangesAsync();
            return Ok(existinghotel.Adapt<HotelDTO>());

        }
        //To delete the hotel details here
        [HttpDelete("{HotelId:guid}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.TenantAdmin}")]  
        public async Task<IActionResult> DeleteHotel(Guid HotelId)
        {
            var hotel = await _hotelRepository.GetByIdAsync(HotelId);
            if (hotel == null)
            {
                return NotFound(new { message = $"Hotel ID{HotelId} not found here" });

            }
            _hotelRepository.Delete(hotel);
            await _hotelRepository.SaveChangesAsync();
            return NoContent();
        }
    }
}
