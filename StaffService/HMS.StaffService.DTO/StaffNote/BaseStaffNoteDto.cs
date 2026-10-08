using System.ComponentModel.DataAnnotations;

namespace HMS.StaffService.DTO.StaffNote
{
    public class BaseStaffNoteDto
    {
        [Required]
        public string Note { get; set; } = string.Empty;
    }
}