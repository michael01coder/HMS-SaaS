using HMS.BillingService.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.BillingService.DTO.PaymentDTO
{
    public class PaymentDTO
    {
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }   
    public string TransactionId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime PaymentDate { get; set; }
    public string? Remarks  { get; set; }
    }
}
