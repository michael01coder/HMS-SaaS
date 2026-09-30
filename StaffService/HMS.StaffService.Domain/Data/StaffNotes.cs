using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.StaffService.Domain.Data
{
    [Table("StaffNotes")]
    public class StaffNotes
    {
        [Key]
        public Guid NoteId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid StaffId { get; set; }

        [Required]
        [Column("NoteText")]
        public string Note { get; set; } = string.Empty;

        public Guid CreatedByStaffId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(StaffId))]
        public virtual Staff? Staff { get; set; }
    }
}