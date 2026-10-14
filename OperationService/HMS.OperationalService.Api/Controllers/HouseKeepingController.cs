using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.Service.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace HMS.OperationalService.Api.Controllers
{
   
    [ApiController]
    [Route("api/[controller]")]
    public class HousekeepingController : ControllerBase
    {
        private readonly IHouseKeepingService _housekeepingService;

        public HousekeepingController(IHouseKeepingService housekeepingService)
        {
            _housekeepingService = housekeepingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            var tasks = await _housekeepingService.GetAllTasksAsync();
            return Ok(tasks);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var task = await _housekeepingService.GetTaskByIdAsync(id);
            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] HouseKeepingTask task)
        {
            var created = await _housekeepingService.CreateTaskAsync(task);
            return CreatedAtAction(nameof(GetTaskById), new { id = created.HousekeepingTaskId }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] HouseKeepingTask task)
        {
            if (id != task.HousekeepingTaskId) return BadRequest("Task ID mismatch.");
            await _housekeepingService.UpdateTaskAsync(task);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            await _housekeepingService.DeleteTaskAsync(id);
            return NoContent();
        }
    }
}

