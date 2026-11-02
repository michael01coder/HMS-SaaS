namespace HMS.AuthService.Service.External
{
    public interface IStaffServiceClient
    {
        Task<string?> CreateStaffAsync(StaffCreationRequest request, CancellationToken cancellationToken = default);
    }

    public class StaffServiceCallException : Exception
    {
        public StaffServiceCallException(string message) : base(message) { }
        public StaffServiceCallException(string message, Exception inner) : base(message, inner) { }
    }
}
