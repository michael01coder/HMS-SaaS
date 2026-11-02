using HMS.BillingService.DTO.InvoiceDTO;

namespace HMS.BillingService.Service.Contracts
{
    public interface IInvoiceService
    {
        Task<InvoiceDTO> CreateInvoiceAsync(CreateInvoiceDTO dto);
        Task<InvoiceDTO> GetByIdAsync(Guid id);
        Task<IEnumerable<InvoiceDTO>> GetAllAsync();
        Task<IEnumerable<InvoiceDTO>> GetByBookingAsync(int bookingId);
        Task<InvoiceDTO> UpdateAsync(Guid id, UpdateInvoiceDTO dto);
        Task DeleteAsync(Guid id);
    }
}
