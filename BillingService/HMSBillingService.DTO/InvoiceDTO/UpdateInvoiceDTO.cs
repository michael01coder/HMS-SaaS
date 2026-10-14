using HMS.BillingService.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.BillingService.DTO.InvoiceDTO
{
    public class UpdateInvoiceDTO
    {
    public decimal TotalAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public InvoiceStatus Status { get; set; }
    public DateTime DueDate { get; set; }
    }
}
