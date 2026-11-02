using System.ComponentModel.DataAnnotations;
using HMS.BillingService.Common.Enums;

namespace HMS.BillingService.DTO.InvoiceDTO
{
    public class UpdateInvoiceDTO
    {
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TaxAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DiscountAmount { get; set; }

        public InvoiceStatus? Status { get; set; }
        public DateTime DueDate { get; set; }
    }
}
