using HMS.AttendanceService.Common.Constants;
using HMS.AttendanceService.DTO.Attendance;
using HMS.AttendanceService.Service.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.AttendanceService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AttendancesController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendancesController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpPost("clock-in")]
        [Authorize(Roles = "HR, Manager, Staff")]
        public async Task<ActionResult<AttendanceDto>> ClockIn([FromBody] ClockInDto dto)
        {
            return Ok(await _attendanceService.ClockInAsync(dto));
        }

        [HttpPost("clock-out")]
        [Authorize(Roles = "HR, Manager, Staff")]
        public async Task<ActionResult<AttendanceDto>> ClockOut([FromBody] ClockOutDto dto)
        {
            return Ok(await _attendanceService.ClockOutAsync(dto));
        }

        [HttpGet("daily")]
        [Authorize(Roles = "HR, Manager")]
        public async Task<ActionResult<IEnumerable<AttendanceDto>>> GetDaily([FromQuery] DateOnly date)
        {
            return Ok(await _attendanceService.GetDailyAttendanceAsync(date));
        }

        [HttpGet("staff/{staffId:guid}")]
        [Authorize(Roles = "HR, Manager, Staff")]
        public async Task<ActionResult<IEnumerable<AttendanceDto>>> GetStaffHistory(
            Guid staffId,
            [FromQuery] DateOnly startDate,
            [FromQuery] DateOnly endDate)
        {
            return Ok(await _attendanceService.GetStaffHistoryAsync(staffId, startDate, endDate));
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public async Task<ActionResult<AttendanceDto>> ManualCreate([FromBody] CreateAttendanceDto dto)
        {
            var result = await _attendanceService.ManualCreateAsync(dto);
            return CreatedAtAction(nameof(GetDaily), new { date = result.AttendanceDate }, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAttendanceDto dto)
        {
            await _attendanceService.UpdateAsync(id, dto);
            return NoContent();
        }
    }
}