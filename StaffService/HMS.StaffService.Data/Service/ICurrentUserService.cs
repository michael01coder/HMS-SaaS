namespace HMS.StaffService.Common.Service
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        Guid? HotelId { get; }
        Guid? StaffId { get; }
        string? Email { get; }
        string? Role { get; }
    }
}