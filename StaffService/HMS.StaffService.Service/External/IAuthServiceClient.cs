namespace HMS.StaffService.Service.External
{
    public interface IAuthServiceClient
    {
        Task SetLoginActiveAsync(Guid staffId, bool isActive, CancellationToken cancellationToken = default);
        Task SetDepartmentAsync(Guid staffId, string? departmentName, CancellationToken cancellationToken = default);
    }
}
