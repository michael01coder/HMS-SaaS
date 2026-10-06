namespace HMS.StaffService.DTO.StaffNote
{
    public class StaffNoteDto : BaseStaffNoteDto
    {
        public Guid NoteId { get; set; }
        public Guid StaffId { get; set; }
        public Guid CreatedByStaffId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}