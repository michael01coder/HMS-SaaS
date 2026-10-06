using System.ComponentModel.DataAnnotations;

namespace HMS.StaffService.DTO.StaffNote
{
    public class CreateStaffNoteDto : BaseStaffNoteDto
    {
        [Required]
        public Guid StaffId { get; set; }
    }
}