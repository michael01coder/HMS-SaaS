using HMS.BillingService.Domain;
using HMS.BillingService.Domain.Data;
using HMS.BillingService.Repository.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.BillingService.Repository
{
    public class PaymentRepository : GenericRepository<Payment , Guid>, IPaymentRepository
    {
        public PaymentRepository(BillingDbContext context) : base(context) { }

        public async Task<IEnumerable<Payment>> GetPaymentsByInvoiceIdAsync(Guid invoiceId)
        {
            return await _dbSet
                .Where(p => p.InvoiceId == invoiceId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }
    }
}
