using HMS.BillingService.DTO.InvoiceDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.BillingService.Service.Contracts
{
    public interface IInvoiceService
    {
        Task<InvoiceDTO> CreateInvoiceAsync(CreateInvoiceDTO dto);
        Task<InvoiceDTO> GetByIdAsync(Guid id);
        Task<IEnumerable<InvoiceDTO>> GetAllAsync();
        Task<InvoiceDTO> UpdateAsync(Guid id, UpdateInvoiceDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
