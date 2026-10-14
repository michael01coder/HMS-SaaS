using HMS.BillingService.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.BillingService.DTO.PaymentDTO
{
    public class UpdatePaymentDTO
    {
        public PaymentStatus Status { get; set; }
        public string? Remarks { get; set; }

    }
}
