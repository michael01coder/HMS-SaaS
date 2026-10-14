using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.BillingService.Common.Enums
{
    public enum InvoiceStatus
    {
        Draft = 1,
        Unpaid = 2,
        Paid = 3,
        Cancelled = 4,
        PartiallyPaid = 5
    }
}
