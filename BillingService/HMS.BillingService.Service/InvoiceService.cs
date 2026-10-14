using HMS.BillingService.Common.Enums;
using HMS.BillingService.Common.Exceptions;
using HMS.BillingService.Domain.Data;
using HMS.BillingService.DTO.InvoiceDTO;
using HMS.BillingService.Repository.Contracts;
using HMS.BillingService.Service.Contracts;

namespace HMS.BillingService.Service
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IGenericRepository<Invoice , Guid> _invoiceRepo;

        public InvoiceService(IGenericRepository<Invoice , Guid> invoiceRepo)
        {
            _invoiceRepo = invoiceRepo;
        }

        public async Task<InvoiceDTO> CreateInvoiceAsync(CreateInvoiceDTO dto)
        {
            var invoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

            var invoice = new Invoice
            {
                BookingId = dto.BookingId,
                GuestId = dto.GuestId,
                InvoiceNumber = invoiceNumber,
                TotalAmount = dto.TotalAmount,
                TaxAmount = dto.TaxAmount,
                DiscountAmount = dto.DiscountAmount,
                PaidAmount = 0,
                Status = InvoiceStatus.Unpaid,
                DueDate = dto.DueDate,
                CreatedAt = DateTime.UtcNow
            };

            await _invoiceRepo.AddAsync(invoice);
            await _invoiceRepo.SaveChangesAsync();

            return MapToDto(invoice);
        }

        public async Task<InvoiceDTO> GetByIdAsync(Guid id)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(id)
                ?? throw new NotFoundException($"Invoice with ID {id} not found.");
            return MapToDto(invoice);
        }

        public async Task<IEnumerable<InvoiceDTO>> GetAllAsync()
        {
            var invoices = await _invoiceRepo.GetAllAsync();
            return invoices.Select(MapToDto);
        }

        public async Task<InvoiceDTO> UpdateAsync(Guid id, UpdateInvoiceDTO dto)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(id)
                ?? throw new NotFoundException($"Invoice with ID {id} not found.");

            invoice.TotalAmount = dto.TotalAmount;
            invoice.TaxAmount = dto.TaxAmount;
            invoice.DiscountAmount = dto.DiscountAmount;
            invoice.Status = dto.Status;
            invoice.DueDate = dto.DueDate;
            invoice.UpdatedAt = DateTime.UtcNow;

            _invoiceRepo.Update(invoice);
            await _invoiceRepo.SaveChangesAsync();

            return MapToDto(invoice);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(id)
                ?? throw new NotFoundException($"Invoice with ID {id} not found.");

            _invoiceRepo.Delete(invoice);
            return await _invoiceRepo.SaveChangesAsync();
        }

        private static InvoiceDTO MapToDto(Invoice i) => new InvoiceDTO
        {
            Id = i.Id,
            BookingId = i.BookingId,
            GuestId = i.GuestId,
            InvoiceNumber = i.InvoiceNumber,
            TotalAmount = i.TotalAmount,
            PaidAmount = i.PaidAmount,
            TaxAmount = i.TaxAmount,
            DiscountAmount = i.DiscountAmount,
            Status = i.Status,
            DueDate = i.DueDate,
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt
        };
    }
}
