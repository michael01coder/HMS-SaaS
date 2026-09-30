using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.StaffService.Domain.Data
{
    [Table("StaffDocuments")]
    public class StaffDocument
    {
        [Key]
        public Guid DocumentId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid StaffId { get; set; }

        [Required]
        [MaxLength(100)]
        public string DocumentType { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string DocumentName { get; set; } = string.Empty;

        public DateTime ExpiryDate { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(500)]
        public string FileUrl { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(StaffId))]
        public virtual Staff? Staff { get; set; }
    }
}