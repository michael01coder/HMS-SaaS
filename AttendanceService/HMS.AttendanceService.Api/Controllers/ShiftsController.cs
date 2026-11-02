using HMS.AttendanceService.DTO.Shift;
using HMS.AttendanceService.Service.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.AttendanceService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShiftsController : ControllerBase
    {
        private readonly IShiftService _shiftService;

        public ShiftsController(IShiftService shiftService)
        {
            _shiftService = shiftService;
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public async Task<ActionResult<ShiftDto>> CreateShift([FromBody] CreateShiftDto dto)
        {
            return Ok(await _shiftService.CreateShiftAsync(dto));
        }

        [HttpGet]
        [Authorize(Roles = "HR, Manager, Staff")]
        public async Task<ActionResult<IEnumerable<ShiftDto>>> GetHotelShifts()
        {
            return Ok(await _shiftService.GetHotelShiftsAsync());
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> UpdateShift(Guid id, [FromBody] UpdateShiftDto dto)
        {
            await _shiftService.UpdateShiftAsync(id, dto);
            return NoContent();
        }

        [HttpPost("allocations")]
        [Authorize(Roles = "HR, Manager")]
        public async Task<ActionResult<ShiftAllocationDto>> AllocateShift([FromBody] CreateShiftAllocationDto dto)
        {
            return Ok(await _shiftService.AllocateShiftAsync(dto));
        }

        [HttpPost("allocations/bulk")]
        [Authorize(Roles = "HR, Manager")]
        public async Task<ActionResult<IEnumerable<ShiftAllocationDto>>> BulkAllocateShifts([FromBody] BulkShiftAllocationDto dto)
        {
            return Ok(await _shiftService.BulkAllocateShiftsAsync(dto));
        }

        [HttpGet("allocations")]
        [Authorize(Roles = "HR, Manager")]
        public async Task<ActionResult<IEnumerable<ShiftAllocationDto>>> GetHotelAllocations(
            [FromQuery] DateOnly startDate,
            [FromQuery] DateOnly endDate)
        {
            return Ok(await _shiftService.GetHotelAllocationsAsync(startDate, endDate));
        }

        [HttpGet("allocations/staff/{staffId:guid}")]
        [Authorize(Roles = "HR, Manager, Staff")]
        public async Task<ActionResult<IEnumerable<ShiftAllocationDto>>> GetStaffAllocations(
            Guid staffId,
            [FromQuery] DateOnly startDate,
            [FromQuery] DateOnly endDate)
        {
            return Ok(await _shiftService.GetStaffAllocationsAsync(staffId, startDate, endDate));
        }

        [HttpPost("swaps")]
        [Authorize(Roles = "HR, Manager, Staff")]
        public async Task<ActionResult<ShiftSwapRequestDto>> CreateSwapRequest([FromBody] CreateShiftSwapRequestDto dto)
        {
            return Ok(await _shiftService.CreateSwapRequestAsync(dto));
        }

        [HttpGet("swaps/pending")]
        [Authorize(Roles = "HR, Manager")]
        public async Task<ActionResult<IEnumerable<ShiftSwapRequestDto>>> GetPendingSwaps()
        {
            return Ok(await _shiftService.GetPendingSwapsAsync());
        }

        [HttpGet("swaps/staff/{staffId:guid}")]
        [Authorize(Roles = "HR, Manager, Staff")]
        public async Task<ActionResult<IEnumerable<ShiftSwapRequestDto>>> GetStaffSwaps(Guid staffId)
        {
            return Ok(await _shiftService.GetStaffSwapsAsync(staffId));
        }

        [HttpPut("swaps/{id:guid}/status")]
        [Authorize(Roles = "HR, Manager")]
        public async Task<ActionResult<ShiftSwapRequestDto>> UpdateSwapStatus(Guid id, [FromBody] UpdateShiftSwapStatusDto dto)
        {
            return Ok(await _shiftService.UpdateSwapStatusAsync(id, dto));
        }
    }
}