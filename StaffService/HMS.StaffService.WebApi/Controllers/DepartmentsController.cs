using HMS.StaffService.DTO.Department;
using HMS.StaffService.Service.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.StaffService.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        [Authorize(Roles = "HR, Manager, Staff")]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetAll()
        {
            return Ok(await _departmentService.GetAllDepartmentsAsync());
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "HR, Manager, Staff")]
        public async Task<ActionResult<DepartmentDto>> GetById(int id)
        {
            return Ok(await _departmentService.GetDepartmentByIdAsync(id));
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public async Task<ActionResult<DepartmentDto>> Create([FromBody] CreateDepartmentDto dto)
        {
            var created = await _departmentService.CreateDepartmentAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.DepartmentId }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Update(int id, [FromBody] BaseDepartmentDto dto)
        {
            await _departmentService.UpdateDepartmentAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Delete(int id)
        {
            await _departmentService.DeleteDepartmentAsync(id);
            return NoContent();
        }
    }
}