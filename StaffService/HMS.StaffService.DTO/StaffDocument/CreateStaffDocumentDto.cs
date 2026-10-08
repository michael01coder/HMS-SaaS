using System.ComponentModel.DataAnnotations;

namespace HMS.StaffService.DTO.StaffDocument
{
    public class CreateStaffDocumentDto
    {
        [Required]
        public Guid StaffId { get; set; }
        [Required]
        public string DocumentType { get; set; } = string.Empty;
        [Required]
        public string DocumentName { get; set; } = string.Empty;
        [Required]
        public string FileUrl { get; set; } = string.Empty;
    }
}
