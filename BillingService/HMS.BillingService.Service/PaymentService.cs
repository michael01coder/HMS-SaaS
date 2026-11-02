using HMS.BillingService.Common.Enums;
using HMS.BillingService.Common.Exceptions;
using HMS.BillingService.Common.Security;
using HMS.BillingService.Domain.Data;
using HMS.BillingService.DTO.PaymentDTO;
using HMS.BillingService.Repository.Contracts;
using HMS.BillingService.Service.Contracts;

namespace HMS.BillingService.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly IGenericRepository<Invoice, Guid> _invoiceRepo;
        private readonly IAccessGuard _guard;

        public PaymentService(IPaymentRepository paymentRepo, IGenericRepository<Invoice, Guid> invoiceRepo, IAccessGuard guard)
        {
            _paymentRepo = paymentRepo;
            _invoiceRepo = invoiceRepo;
            _guard = guard;
        }

        public async Task<PaymentDTO> ProcessPaymentAsync(CreatePaymentDTO dto)
        {
            _guard.Require(Module.Billing);

            if (dto.Amount <= 0)
                throw new BadRequestException("Payment amount must be greater than zero.");

            var invoice = await GetOwnedInvoiceAsync(dto.InvoiceId);

            if (invoice.Status is InvoiceStatus.Paid or InvoiceStatus.Cancelled or InvoiceStatus.Draft)
                throw new BadRequestException($"A {invoice.Status} invoice cannot take payments.");

            var remaining = InvoiceMath.NetAmount(invoice) - invoice.PaidAmount;
            if (dto.Amount > remaining)
                throw new BadRequestException($"Payment amount ({dto.Amount}) exceeds the remaining balance ({remaining}).");

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                HotelId = invoice.HotelId,
                InvoiceId = invoice.Id,
                TransactionId = $"TXN-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}",
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod,
                Status = PaymentStatus.Completed,
                PaymentDate = DateTime.UtcNow,
                Remarks = dto.Remarks
            };

            await _paymentRepo.AddAsync(payment);

            invoice.PaidAmount += dto.Amount;
            invoice.Status = InvoiceMath.StatusFor(invoice);
            invoice.UpdatedAt = DateTime.UtcNow;
            _invoiceRepo.Update(invoice);

            await _paymentRepo.SaveChangesAsync();
            return MapToDto(payment);
        }

        public async Task<PaymentDTO> GetByIdAsync(Guid id)
        {
            _guard.Require(Module.Billing);
            return MapToDto(await GetOwnedAsync(id));
        }

        public async Task<IEnumerable<PaymentDTO>> GetAllAsync()
        {
            _guard.Require(Module.Billing);
            var hotelId = _guard.HotelId;
            var payments = await _paymentRepo.FindAsync(p => p.HotelId == hotelId);
            return payments.OrderByDescending(p => p.PaymentDate).Select(MapToDto);
        }

        public async Task<IEnumerable<PaymentDTO>> GetByInvoiceIdAsync(Guid invoiceId)
        {
            _guard.Require(Module.Billing);
            await GetOwnedInvoiceAsync(invoiceId);
            var payments = await _paymentRepo.GetPaymentsByInvoiceIdAsync(invoiceId);
            return payments.Select(MapToDto);
        }

        public async Task<PaymentDTO> UpdateStatusAsync(Guid id, UpdatePaymentDTO dto)
        {
            _guard.Require(Module.Billing);
            var payment = await GetOwnedAsync(id);
            var invoice = await GetOwnedInvoiceAsync(payment.InvoiceId);

            if (payment.Status != dto.Status)
            {
                var wasCounted = payment.Status == PaymentStatus.Completed;
                var willCount = dto.Status == PaymentStatus.Completed;

                if (wasCounted && !willCount) invoice.PaidAmount -= payment.Amount;
                if (!wasCounted && willCount)
                {
                    if (invoice.PaidAmount + payment.Amount > InvoiceMath.NetAmount(invoice))
                        throw new BadRequestException("Completing this payment would exceed the invoice total.");
                    invoice.PaidAmount += payment.Amount;
                }

                payment.Status = dto.Status;
                if (invoice.Status != InvoiceStatus.Cancelled)
                    invoice.Status = InvoiceMath.StatusFor(invoice);
                invoice.UpdatedAt = DateTime.UtcNow;
                _invoiceRepo.Update(invoice);
            }

            if (!string.IsNullOrWhiteSpace(dto.Remarks))
                payment.Remarks = dto.Remarks;

            _paymentRepo.Update(payment);
            await _paymentRepo.SaveChangesAsync();
            return MapToDto(payment);
        }

        private async Task<Payment> GetOwnedAsync(Guid id)
        {
            var payment = await _paymentRepo.GetByIdAsync(id)
                ?? throw new NotFoundException($"Payment with ID {id} not found.");
            if (payment.HotelId != _guard.HotelId)
                throw new ForbiddenException("Access denied.");
            return payment;
        }

        private async Task<Invoice> GetOwnedInvoiceAsync(Guid id)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(id)
                ?? throw new NotFoundException($"Invoice with ID {id} not found.");
            if (invoice.HotelId != _guard.HotelId)
                throw new ForbiddenException("Access denied.");
            return invoice;
        }

        private static PaymentDTO MapToDto(Payment p) => new()
        {
            Id = p.Id,
            HotelId = p.HotelId,
            InvoiceId = p.InvoiceId,
            TransactionId = p.TransactionId,
            Amount = p.Amount,
            PaymentMethod = p.PaymentMethod,
            Status = p.Status,
            PaymentDate = p.PaymentDate,
            Remarks = p.Remarks
        };
    }
}
