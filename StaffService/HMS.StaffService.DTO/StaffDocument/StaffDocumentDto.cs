namespace HMS.StaffService.DTO.StaffDocument
{
    public class StaffDocumentDto : CreateStaffDocumentDto
    {
        public Guid DocumentId { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
