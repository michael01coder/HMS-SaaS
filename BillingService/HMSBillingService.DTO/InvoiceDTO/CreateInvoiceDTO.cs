using System.ComponentModel.DataAnnotations;

namespace HMS.BillingService.DTO.InvoiceDTO
{
    public class CreateInvoiceDTO
    {
        [Range(1, int.MaxValue)]
        public int BookingId { get; set; }

        [Required]
        public Guid GuestId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TaxAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DiscountAmount { get; set; }

        public DateTime DueDate { get; set; }
    }
}
