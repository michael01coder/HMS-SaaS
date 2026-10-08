namespace HMS.AuthService.Service.External
{
    public interface IStaffServiceClient
    {
        /// <summary>
        /// Calls StaffService's internal endpoint to create the matching
        /// Staff row for a newly registered HR/Manager/Staff user. Throws
        /// StaffServiceCallException on any failure (non-success response,
        /// timeout, or connection failure) — AuthService.RegisterAsync
        /// catches that and rolls back the Identity user it just created.
        /// </summary>
        Task CreateStaffAsync(StaffCreationRequest request, CancellationToken cancellationToken = default);
    }

    public class StaffServiceCallException : Exception
    {
        public StaffServiceCallException(string message) : base(message) { }
        public StaffServiceCallException(string message, Exception inner) : base(message, inner) { }
    }
}
