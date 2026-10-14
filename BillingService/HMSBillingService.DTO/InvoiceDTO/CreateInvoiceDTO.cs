using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.BillingService.DTO.InvoiceDTO
{
    public class CreateInvoiceDTO
    {
    public Guid BookingId { get; set; }
    public Guid GuestId { get; set; }
    public decimal TotalAmount { get; set; }
    public  decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public DateTime DueDate { get; set; }
    }
}
