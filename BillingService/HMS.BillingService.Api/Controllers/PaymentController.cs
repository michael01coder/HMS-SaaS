using HMS.BillingService.DTO.PaymentDTO;
using HMS.BillingService.Service.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace HMS.BillingService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _paymentService.GetAllAsync());
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await _paymentService.GetByIdAsync(id));
        }

        [HttpGet("invoice/{invoiceId:guid}")]
        public async Task<IActionResult> GetByInvoiceId(Guid invoiceId)
        {
            return Ok(await _paymentService.GetByInvoiceIdAsync(invoiceId));
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment([FromBody] CreatePaymentDTO dto)
        {
            var result = await _paymentService.ProcessPaymentAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdatePaymentDTO dto)
        {
            return Ok(await _paymentService.UpdateStatusAsync(id, dto));
        }
    }
}
