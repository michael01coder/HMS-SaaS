using HMS.OperationalService.Common.Exceptions;

namespace HMS.OperationalService.Common.Security
{
    public enum Module
    {
        FrontDesk,
        Housekeeping,
        Maintenance,
        Billing
    }

    public interface IAccessGuard
    {
        Guid HotelId { get; }
        void Require(Module module);
        void RequireAny(params Module[] modules);
        void RequireRole(params string[] roles);
    }

    public class AccessGuard : IAccessGuard
    {
        private readonly ICurrentUser _user;

        public AccessGuard(ICurrentUser user) => _user = user;

        public Guid HotelId => _user.HotelId ?? throw new ForbiddenException("This action needs a hotel-scoped account.");

        public void RequireAny(params Module[] modules)
        {
            ForbiddenException? last = null;
            foreach (var module in modules)
            {
                try
                {
                    Require(module);
                    return;
                }
                catch (ForbiddenException ex)
                {
                    last = ex;
                }
            }
            throw last ?? new ForbiddenException("Your department cannot use this module.");
        }

        public void Require(Module module)
        {
            _ = HotelId;
            if (_user.Role is "HotelOwner" or "HR") return;
            if (_user.Role is not ("Manager" or "Staff"))
                throw new ForbiddenException("Your role cannot use this module.");

            var department = _user.Department ?? string.Empty;
            var allowed = module switch
            {
                Module.FrontDesk => Matches(department, "front", "reception", "guest", "concierge"),
                Module.Housekeeping => Matches(department, "house", "laundry", "cleaning", "room"),
                Module.Maintenance => Matches(department, "maint", "engineer", "technic", "facilit"),
                Module.Billing => Matches(department, "front", "reception", "account", "finance", "billing"),
                _ => false
            };

            if (!allowed)
                throw new ForbiddenException("Your department cannot use this module.");
        }

        public void RequireRole(params string[] roles)
        {
            if (!roles.Contains(_user.Role))
                throw new ForbiddenException("Your role cannot perform this action.");
        }

        private static bool Matches(string department, params string[] keys) =>
            keys.Any(k => department.Contains(k, StringComparison.OrdinalIgnoreCase));
    }
}
