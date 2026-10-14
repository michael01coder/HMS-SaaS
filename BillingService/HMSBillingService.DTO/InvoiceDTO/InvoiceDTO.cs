using HMS.BillingService.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.BillingService.DTO.InvoiceDTO
{
    public class InvoiceDTO
    {
       

    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Guid GuestId { get; set; }
    public string InvoiceNumber { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public InvoiceStatus Status { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    }
}
