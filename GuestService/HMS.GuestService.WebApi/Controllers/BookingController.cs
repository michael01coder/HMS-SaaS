using HMS.GuestService.DTO.BookingDTO;
using HMS.GuestService.Service.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.GuestService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "HotelOwner, HR, Manager, Staff")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService) => _bookingService = bookingService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _bookingService.GetAllBookingsAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _bookingService.GetBookingByIdAsync(id));

        [HttpGet("guest/{guestId:guid}")]
        public async Task<IActionResult> GetByGuestId(Guid guestId) => Ok(await _bookingService.GetBookingsByGuestAsync(guestId));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookingDTO dto)
        {
            var booking = await _bookingService.CreateBookingAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = booking.BookingId }, booking);
        }

        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateBookingDTO dto)
        {
            await _bookingService.UpdateBookingStatusAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Cancel(int id)
        {
            await _bookingService.CancelBookingAsync(id);
            return NoContent();
        }
    }
}
