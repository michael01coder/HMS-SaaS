using HMS.BillingService.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.BillingService.Repository.Contracts
{
    public interface IPaymentRepository : IGenericRepository<Payment , Guid>
    {
        Task<IEnumerable<Payment>> GetPaymentsByInvoiceIdAsync(Guid invoiceId);
    }
}
