using HMS.BillingService.Common.Enums;
using HMS.BillingService.Common.Exceptions;
using HMS.BillingService.Domain.Data;
using HMS.BillingService.DTO.PaymentDTO;
using HMS.BillingService.Repository.Contracts;
using HMS.BillingService.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.BillingService.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly IGenericRepository<Invoice , Guid> _invoiceRepo;

        public PaymentService(
            IPaymentRepository paymentRepo,
            IGenericRepository<Invoice , Guid> invoiceRepo)
        {
            _paymentRepo = paymentRepo;
            _invoiceRepo = invoiceRepo;
        }

        public async Task<PaymentDTO> ProcessPaymentAsync(CreatePaymentDTO dto)
        {
            if (dto.Amount <= 0)
            {
                throw new BadRequestException("Payment amount must be greater than zero.");
            }

            var invoice = await _invoiceRepo.GetByIdAsync(dto.InvoiceId)
                ?? throw new NotFoundException($"Invoice with ID {dto.InvoiceId} not found.");

            if (invoice.Status == InvoiceStatus.Paid)
            {
                throw new BadRequestException("Invoice is already fully paid.");
            }

            var remainingAmount = invoice.TotalAmount - invoice.PaidAmount;
            if (dto.Amount > remainingAmount)
            {
                throw new BadRequestException($"Payment amount ({dto.Amount}) exceeds remaining unpaid invoice balance ({remainingAmount}).");
            }

            var transactionId = $"TXN-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

            var payment = new Payment
            {
                InvoiceId = dto.InvoiceId,
                TransactionId = transactionId,
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod,
                Status = PaymentStatus.Completed,
                PaymentDate = DateTime.UtcNow,
                Remarks = dto.Remarks
            };

            await _paymentRepo.AddAsync(payment);

            // Auto-update Invoice Paid Amount & Status
            invoice.PaidAmount += dto.Amount;
            invoice.Status = invoice.PaidAmount >= invoice.TotalAmount
                ? InvoiceStatus.Paid
                : InvoiceStatus.PartiallyPaid;
            invoice.UpdatedAt = DateTime.UtcNow;

            _invoiceRepo.Update(invoice);

            await _paymentRepo.SaveChangesAsync();

            return MapToDto(payment);
        }

        public async Task<PaymentDTO> GetByIdAsync(Guid id)
        {
            var payment = await _paymentRepo.GetByIdAsync(id)
                ?? throw new NotFoundException($"Payment with ID {id} not found.");

            return MapToDto(payment);
        }

        public async Task<IEnumerable<PaymentDTO>> GetAllAsync()
        {
            var payments = await _paymentRepo.GetAllAsync();
            return payments.Select(MapToDto);
        }

        public async Task<IEnumerable<PaymentDTO>> GetByInvoiceIdAsync(Guid invoiceId)
        {
            var payments = await _paymentRepo.GetPaymentsByInvoiceIdAsync(invoiceId);
            return payments.Select(MapToDto);
        }

        public async Task<PaymentDTO> UpdateStatusAsync(Guid id, UpdatePaymentDTO dto)
        {
            var payment = await _paymentRepo.GetByIdAsync(id)
                ?? throw new NotFoundException($"Payment with ID {id} not found.");

            payment.Status = dto.Status;
            if (!string.IsNullOrWhiteSpace(dto.Remarks))
            {
                payment.Remarks = dto.Remarks;
            }

            _paymentRepo.Update(payment);
            await _paymentRepo.SaveChangesAsync();

            return MapToDto(payment);
        }

        private static PaymentDTO MapToDto(Payment p) => new PaymentDTO
        {
            Id = p.Id,
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
