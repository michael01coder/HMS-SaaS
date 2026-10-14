using HMS.BillingService.DTO.PaymentDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.BillingService.Service.Contracts
{
    public interface IPaymentService
    {
        Task<PaymentDTO> ProcessPaymentAsync(CreatePaymentDTO dto);
        Task<PaymentDTO> GetByIdAsync(Guid id);
        Task<IEnumerable<PaymentDTO>> GetAllAsync();
        Task<IEnumerable<PaymentDTO>> GetByInvoiceIdAsync(Guid invoiceId);
        Task<PaymentDTO> UpdateStatusAsync(Guid id, UpdatePaymentDTO dto);
    }
}
