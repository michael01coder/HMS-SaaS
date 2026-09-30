using HMS.StaffService.DTO.StaffDocument;
using HMS.StaffService.Service.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.StaffService.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StaffDocumentsController : ControllerBase
    {
        private readonly IStaffDocumentService _documentService;

        public StaffDocumentsController(IStaffDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpGet]
        [Authorize(Roles = "HR, Manager")]
        public async Task<ActionResult<IEnumerable<StaffDocumentDto>>> GetAll()
        {
            return Ok(await _documentService.GetAllAsync());
        }

        [HttpGet("staff/{staffId:guid}")]
        [Authorize(Roles = "HR, Manager, Staff")]
        public async Task<ActionResult<IEnumerable<StaffDocumentDto>>> GetByStaffId(Guid staffId)
        {
            return Ok(await _documentService.GetByStaffIdAsync(staffId));
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "HR, Manager, Staff")]
        public async Task<ActionResult<StaffDocumentDto>> GetById(Guid id)
        {
            return Ok(await _documentService.GetByIdAsync(id));
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public async Task<ActionResult<StaffDocumentDto>> Create([FromBody] CreateStaffDocumentDto dto)
        {
            var created = await _documentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.DocumentId }, created);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStaffDocumentDto dto)
        {
            await _documentService.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _documentService.DeleteAsync(id);
            return NoContent();
        }
    }
}