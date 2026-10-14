using HMS.BillingService.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.BillingService.DTO.PaymentDTO
{
    public class CreatePaymentDTO
    {
    public Guid InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? Remarks  { get; set; }
    }
}
