namespace HMS.AuthService.Service.Contracts
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }

        string Role { get; }

        Guid TenantId { get; }

        Guid HotelId { get; }

        Guid StaffId { get; }

        string Email { get; }
    }
}