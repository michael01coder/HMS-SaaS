using HMS.AttendanceService.DTO.Anomaly;
using HMS.AttendanceService.Service.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.AttendanceService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AttendanceAnomaliesController : ControllerBase
    {
        private readonly IAttendanceAnomalyService _anomalyService;

        public AttendanceAnomaliesController(IAttendanceAnomalyService anomalyService)
        {
            _anomalyService = anomalyService;
        }

        [HttpGet("pending")]
        [Authorize(Roles = "HR, Manager")]
        public async Task<ActionResult<IEnumerable<AttendanceAnomalyDto>>> GetPending()
        {
            return Ok(await _anomalyService.GetPendingAnomaliesForHotelAsync());
        }

        [HttpGet("attendance/{attendanceId:guid}")]
        [Authorize(Roles = "HR, Manager, Staff")]
        public async Task<ActionResult<IEnumerable<AttendanceAnomalyDto>>> GetByAttendance(Guid attendanceId)
        {
            return Ok(await _anomalyService.GetAnomaliesByAttendanceIdAsync(attendanceId));
        }

        [HttpGet("staff/{staffId:guid}")]
        [Authorize(Roles = "HR, Manager, Staff")]
        public async Task<ActionResult<IEnumerable<AttendanceAnomalyDto>>> GetByStaff(Guid staffId)
        {
            return Ok(await _anomalyService.GetAnomaliesByStaffIdAsync(staffId));
        }

        [HttpPost]
        [Authorize(Roles = "HR, Manager")]
        public async Task<ActionResult<AttendanceAnomalyDto>> Create([FromBody] CreateAttendanceAnomalyDto dto)
        {
            var created = await _anomalyService.CreateAnomalyAsync(dto);
            return CreatedAtAction(nameof(GetByAttendance), new { attendanceId = created.AttendanceId }, created);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "HR, Manager")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAttendanceAnomalyDto dto)
        {
            await _anomalyService.UpdateAnomalyAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _anomalyService.DeleteAnomalyAsync(id);
            return NoContent();
        }
    }
}