
using HMS.GuestService.DTO.ReservationDTO;
using HMS.GuestService.Service.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace HMS.GuestService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var reservation = await _reservationService.GetReservationByIdAsync(id);
            return Ok(reservation);
        }

        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var reservation = await _reservationService.GetByCodeAsync(code);
            return Ok(reservation);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReservationDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reservation = await _reservationService.CreateReservationAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = reservation.ReservationId }, reservation);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Cancel(int id)
        {
            await _reservationService.CancelReservationAsync(id);
            return NoContent();
        }
    }
}