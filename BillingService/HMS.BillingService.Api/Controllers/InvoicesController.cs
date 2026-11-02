using HMS.BillingService.DTO.InvoiceDTO;
using HMS.BillingService.Service.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.BillingService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "HotelOwner, HR, Manager, Staff")]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoicesController(IInvoiceService invoiceService) => _invoiceService = invoiceService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _invoiceService.GetAllAsync());

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id) => Ok(await _invoiceService.GetByIdAsync(id));

        [HttpGet("booking/{bookingId:int}")]
        public async Task<IActionResult> GetByBooking(int bookingId) => Ok(await _invoiceService.GetByBookingAsync(bookingId));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInvoiceDTO dto)
        {
            var created = await _invoiceService.CreateInvoiceAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateInvoiceDTO dto) =>
            Ok(await _invoiceService.UpdateAsync(id, dto));

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _invoiceService.DeleteAsync(id);
            return NoContent();
        }
    }
}
