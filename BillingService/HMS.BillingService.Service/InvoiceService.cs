using HMS.BillingService.Common.Enums;
using HMS.BillingService.Common.Exceptions;
using HMS.BillingService.Common.Security;
using HMS.BillingService.Domain.Data;
using HMS.BillingService.DTO.InvoiceDTO;
using HMS.BillingService.Repository.Contracts;
using HMS.BillingService.Service.Contracts;

namespace HMS.BillingService.Service
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IGenericRepository<Invoice, Guid> _invoiceRepo;
        private readonly IPaymentRepository _paymentRepo;
        private readonly IAccessGuard _guard;

        public InvoiceService(IGenericRepository<Invoice, Guid> invoiceRepo, IPaymentRepository paymentRepo, IAccessGuard guard)
        {
            _invoiceRepo = invoiceRepo;
            _paymentRepo = paymentRepo;
            _guard = guard;
        }

        public async Task<InvoiceDTO> CreateInvoiceAsync(CreateInvoiceDTO dto)
        {
            _guard.Require(Module.Billing);

            if (dto.DiscountAmount > dto.TotalAmount + dto.TaxAmount)
                throw new BadRequestException("Discount cannot exceed the invoice total.");

            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                HotelId = _guard.HotelId,
                BookingId = dto.BookingId,
                GuestId = dto.GuestId,
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}",
                TotalAmount = dto.TotalAmount,
                TaxAmount = dto.TaxAmount,
                DiscountAmount = dto.DiscountAmount,
                PaidAmount = 0,
                Status = InvoiceStatus.Unpaid,
                DueDate = AsUtc(dto.DueDate),
                CreatedAt = DateTime.UtcNow
            };

            await _invoiceRepo.AddAsync(invoice);
            await _invoiceRepo.SaveChangesAsync();
            return MapToDto(invoice);
        }

        public async Task<InvoiceDTO> GetByIdAsync(Guid id)
        {
            _guard.Require(Module.Billing);
            return MapToDto(await GetOwnedAsync(id));
        }

        public async Task<IEnumerable<InvoiceDTO>> GetAllAsync()
        {
            _guard.Require(Module.Billing);
            var hotelId = _guard.HotelId;
            var invoices = await _invoiceRepo.FindAsync(i => i.HotelId == hotelId);
            return invoices.OrderByDescending(i => i.CreatedAt).Select(MapToDto);
        }

        public async Task<IEnumerable<InvoiceDTO>> GetByBookingAsync(int bookingId)
        {
            _guard.Require(Module.Billing);
            var hotelId = _guard.HotelId;
            var invoices = await _invoiceRepo.FindAsync(i => i.HotelId == hotelId && i.BookingId == bookingId);
            return invoices.OrderByDescending(i => i.CreatedAt).Select(MapToDto);
        }

        public async Task<InvoiceDTO> UpdateAsync(Guid id, UpdateInvoiceDTO dto)
        {
            _guard.Require(Module.Billing);
            var invoice = await GetOwnedAsync(id);

            if (invoice.Status == InvoiceStatus.Cancelled)
                throw new BadRequestException("A cancelled invoice cannot be edited.");

            var net = dto.TotalAmount + dto.TaxAmount - dto.DiscountAmount;
            if (net < invoice.PaidAmount)
                throw new BadRequestException("The invoice total cannot be lower than the amount already paid.");

            invoice.TotalAmount = dto.TotalAmount;
            invoice.TaxAmount = dto.TaxAmount;
            invoice.DiscountAmount = dto.DiscountAmount;
            invoice.DueDate = AsUtc(dto.DueDate);

            if (dto.Status == InvoiceStatus.Cancelled)
            {
                if (invoice.PaidAmount > 0)
                    throw new BadRequestException("Refund the payments before cancelling this invoice.");
                invoice.Status = InvoiceStatus.Cancelled;
            }
            else
            {
                invoice.Status = InvoiceMath.StatusFor(invoice);
            }

            invoice.UpdatedAt = DateTime.UtcNow;
            _invoiceRepo.Update(invoice);
            await _invoiceRepo.SaveChangesAsync();
            return MapToDto(invoice);
        }

        public async Task DeleteAsync(Guid id)
        {
            _guard.RequireRole("HR", "HotelOwner");
            var invoice = await GetOwnedAsync(id);

            if ((await _paymentRepo.GetPaymentsByInvoiceIdAsync(id)).Any())
                throw new BadRequestException("An invoice with payments cannot be deleted.");

            _invoiceRepo.Delete(invoice);
            await _invoiceRepo.SaveChangesAsync();
        }

        private async Task<Invoice> GetOwnedAsync(Guid id)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(id)
                ?? throw new NotFoundException($"Invoice with ID {id} not found.");
            if (invoice.HotelId != _guard.HotelId)
                throw new ForbiddenException("Access denied.");
            return invoice;
        }

        private static DateTime AsUtc(DateTime value) => value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };

        internal static InvoiceDTO MapToDto(Invoice i)
        {
            var net = i.TotalAmount + i.TaxAmount - i.DiscountAmount;
            return new InvoiceDTO
            {
                Id = i.Id,
                HotelId = i.HotelId,
                BookingId = i.BookingId,
                GuestId = i.GuestId,
                InvoiceNumber = i.InvoiceNumber,
                TotalAmount = i.TotalAmount,
                PaidAmount = i.PaidAmount,
                BalanceAmount = Math.Max(0, net - i.PaidAmount),
                TaxAmount = i.TaxAmount,
                DiscountAmount = i.DiscountAmount,
                Status = i.Status,
                IsOverdue = i.Status is InvoiceStatus.Unpaid or InvoiceStatus.PartiallyPaid && i.DueDate < DateTime.UtcNow,
                DueDate = i.DueDate,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt
            };
        }
    }

    internal static class InvoiceMath
    {
        public static decimal NetAmount(Invoice i) => i.TotalAmount + i.TaxAmount - i.DiscountAmount;

        public static InvoiceStatus StatusFor(Invoice i)
        {
            if (i.PaidAmount <= 0) return i.Status == InvoiceStatus.Draft ? InvoiceStatus.Draft : InvoiceStatus.Unpaid;
            return i.PaidAmount >= NetAmount(i) ? InvoiceStatus.Paid : InvoiceStatus.PartiallyPaid;
        }
    }
}
