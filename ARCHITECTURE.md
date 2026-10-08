# Architecture — structure & role model

This explains the two big changes made in this pass: every service now
follows one structure, and the role model is unified and actually enforced.

## 1. The shared structure

Every one of the five real services (Auth, Tenant, Hotel, RoomManagement,
Staff) now has exactly six projects, in this order of dependency:

```
{Service}.Common      — exceptions, Roles constants, ICurrentUserService
       ↓
{Service}.Domain      — entities + DbContext + Migrations, together
       ↓
{Service}.DTO         — one folder per entity: BaseXDto, CreateXDto, UpdateXDto, XDto
       ↓
{Service}.Repository  — generic repo (Mapster-integrated) + one per entity
       ↓
{Service}.Service     — business logic + authorization scoping. Controllers
                         never touch a Repository directly anymore.
       ↓
{Service}.Api/WebApi  — Controllers, Middleware/ExceptionMiddleware,
                         Configurations/MapsterConfig, Program.cs, .env
```

This is StaffService's own shape (shown in your screenshots) — Tenant,
Hotel, and RoomManagement were rebuilt to match it exactly, including two
fixes applied everywhere at once:

- **`DbContext` moved into `Domain`.** It used to live in a separate
  `*.Data` project in your three services (Staff never had this split).
  That project is gone; its `DbContext` and `Migrations/` folder now live
  directly in `*.Domain`, same as Staff. **This changes the `--project`
  path for every `dotnet ef` command** — see below.
- **`ExceptionMiddleware` only mapped `NotFound`/`BadRequest`.**
  `ForbiddenException` and `UnauthorizedException` — both thrown
  deliberately by the Service layer for scope checks — fell through to a
  bare 500. Fixed in all five services, including Staff's original copy.
  Staff's middleware also wasn't registered in `Program.cs` at all (the
  class existed, `app.UseMiddleware<ExceptionMiddleware>()` was never
  called) — added.

Folder naming in Staff itself was also cleaned up: `HMS.StaffService.Data`
(which actually contained the `Common` project) and `HMS.StaffService.Repo`
are renamed to `HMS.StaffService.Common` and `HMS.StaffService.Repository`,
matching their own `.csproj` names.

## 2. The role model

### The six roles

One `UserRole` enum, duplicated as a string `Roles` class in every service's
`Common` project (no shared library between services, same as everywhere
else in this solution):

| Role | Has TenantId | Has HotelId | Scope |
|---|---|---|---|
| `SuperAdmin` | — | — | Everything, platform-wide |
| `TenantAdmin` | required | — | Everything under their tenant |
| `HotelOwner` | required | required | Their one hotel |
| `HR` | required | required | Staff & departments at their hotel |
| `Manager` | required | required | Day-to-day ops at their hotel |
| `Staff` | required | required | General staff at their hotel |

**There is no `FrontDeskStaff` role anywhere.** It's a `Staff` (or
`Manager`) user whose `Department` is `"Front Desk"` — a property, not a
role. See `RoomService.UpdateRoomStatusAsync` in `RoomManagementService`
for the one concrete example of checking it:

```csharp
bool allowed =
    role is Roles.SuperAdmin or Roles.TenantAdmin ||
    (role == Roles.Manager && sameHotel) ||
    (role == Roles.Staff && sameHotel &&
        string.Equals(_currentUserService.Department, "Front Desk", StringComparison.OrdinalIgnoreCase));
```

Why a property instead of a role: a role answers "what can this person do
in general"; a department answers "which part of the building are they
in." Front Desk isn't a different permission level from other Staff — it's
the same `Staff` permissions, just a different team. Modeling it as a role
would mean a seventh value that every `[Authorize(Roles=...)]` list has to
remember to include, for something that's really just a filter. Any future
Booking/Billing endpoint that needs "front desk only" copies that same
`Department == "Front Desk"` check — it isn't special-cased anywhere else.

### Why this needed fixing in Auth specifically

Three real bugs, now fixed in `AuthService`:

1. **`ApplicationUser.HotelId`/`StaffId` were non-nullable `Guid`.** A
   `SuperAdmin` or `TenantAdmin` — who legitimately have no hotel — got a
   `HotelId` of `00000000-0000-0000-0000-000000000000` instead of "no
   value". `JwtTokenService` used to always emit a `hotelId` claim, even
   that fake one. Now `Guid?` throughout, and the claim is only added to
   the token when the value is actually set.
2. **There was no `TenantId` anywhere** — not on `ApplicationUser`, not on
   `RegisterDto`, not on the JWT. A `TenantAdmin` had no way to express
   *which* tenant they administer. Added end to end.
3. **Nothing validated that a role had the right ids.** You could register
   a `"Staff"` with no `HotelId` and nothing would stop you — until that
   token hit a service expecting one. `AuthService.RegisterAsync` now has
   an explicit `ValidateScopeForRole` check that rejects bad combinations
   at registration time with a clear 400, instead of a confusing failure
   three services later.

A smaller, related fix: `RefreshToken` was keyed on `StaffId`, which is
exactly the field that's now nullable for `SuperAdmin`/`TenantAdmin` — so a
platform admin's refresh token had no valid key to correlate against. It's
keyed on `UserId` (the Identity user's own id, always present for every
role) now instead.

### How each service enforces it

Every `{Service}.Service` class takes `ICurrentUserService` and does one of
two things, visible by example:

- **Tenant-tier scoping** (`TenantService.TenantService`): `SuperAdmin`
  bypasses the check; everyone else must have
  `_currentUserService.TenantId == <the tenant they're touching>`.
- **Hotel-tier scoping** (`HotelService.HotelService`,
  `RoomManagementService.*Service`): `SuperAdmin`/`TenantAdmin` bypass;
  `HotelOwner`/`Manager` must have `_currentUserService.HotelId == <the
  hotel they're touching>`.

Controllers only decide which roles are allowed to *reach* an action at
all (`[Authorize(Roles = "...")]`); the Service layer decides whether
*this specific* tenant/hotel is theirs to touch. That split is why the
scope checks live in `HotelService.cs`, not `HotelsController.cs` — the
same check needs to run whether the call comes from a controller, a
background job, or (later) another service calling in directly.

## 3. Running migrations — paths changed for Tenant/Hotel/Room

Because `DbContext` moved from `*.Data` into `*.Domain`, the `--project`
flag changes for three of the five services. **Auth and Staff are
unaffected** (their `DbContext` was always inside `Domain`).

```bash
# Auth — still no migration exists yet
cd AuthService/HMS.AuthService.Api
dotnet ef migrations add InitialIdentity --project ../HMS.AuthService.Domain --startup-project .
dotnet ef database update --project ../HMS.AuthService.Domain --startup-project .

# Staff — migration already exists, just apply it
cd ../../StaffService/HMS.StaffService.WebApi
dotnet ef database update --project ../HMS.StaffService.Domain --startup-project .

# Tenant — existing migration, NEW project path (.Domain, not .Data)
cd ../../TenantService/HMSTenantService.Api
dotnet ef database update --project ../HMSTenant.Domain --startup-project .

# Hotel — existing migration + the new Amenities/HotelAmenities tables
cd ../../HotelService/HMSHotelServices.Api
dotnet ef migrations add AddAmenities --project ../HMSHotel.Domain --startup-project .
dotnet ef database update --project ../HMSHotel.Domain --startup-project .

# Room Management — existing migration, NEW project path
cd ../../RoomManagementService/RoomManagementService.Api
dotnet ef database update --project ../HMSRoomManagement.Domain --startup-project .
```

## 4. What to test, in order

1. **Auth registration for each tier**, confirming the new validation fires:
   - `SuperAdmin` with a `tenantId` or `hotelId` set → expect 400.
   - `TenantAdmin` with no `tenantId` → expect 400.
   - `HotelOwner`/`HR`/`Manager`/`Staff` with no `hotelId` → expect 400.
   - Each with the right ids → expect 200 + a token.
2. **Decode a `TenantAdmin` token** at jwt.io and confirm it has a
   `tenantId` claim and no `hotelId` claim (not an empty-GUID one).
3. **Tenant scoping**: as `TenantAdmin` A, `GET /api/tenants/{tenant B's id}`
   → expect 403 (`ForbiddenException` → now correctly mapped, not 500).
4. **Hotel scoping**: same shape, as `HotelOwner` against a hotel that isn't
   theirs.
5. **The Front Desk department check**: register a `Staff` with
   `departmentName: "Front Desk"`, confirm `PATCH /api/rooms/{id}/status`
   succeeds; register one with a different department, confirm 403.

## 5. Auth → Staff Service connection (the flagged gap, now fixed)

`RegisterDto` always collected `firstName`/`lastName`/`phone`/`departmentId`,
but nothing persisted them and nothing ever created the matching `Staff`
row for the `StaffId` Auth generates. Fixed end to end:

- **`ApplicationUser` now persists `FirstName`/`LastName`/`Phone`** — they
  were captured and silently dropped before.
- **`RegisterAsync` calls StaffService directly** (not through the
  gateway — the same way the gateway's own cluster destinations reach
  each service) right after the Identity user and role are created, for
  `HR`/`Manager`/`Staff` only. `HotelOwner` keeps a `StaffId` on their
  token for claim consistency but gets no `Staff` row — there's no
  `DepartmentId` that makes sense for an owner, and `Staff.DepartmentId`
  is a required FK. `SuperAdmin`/`TenantAdmin` have no `StaffId` at all.
- **No distributed transaction exists across the two databases**, so this
  is a compensating-action (saga) pattern: if StaffService rejects the
  call or is unreachable, `AuthService.RegisterAsync` deletes the Identity
  user it just created and the whole registration fails with a clear 400.
  The two services can't end up with an Identity user that has no
  matching Staff row, or vice versa.
- **The call can't use a normal JWT** — no token exists yet for a user
  mid-registration. Trust instead comes from a shared secret:
  `AuthService` sends `X-Internal-Api-Key` on every call to
  `StaffService`'s `POST /api/internal/staff`, which checks it against
  its own `INTERNAL_API_KEY` — **both `.env` files must hold the exact
  same value**, or every HR/Manager/Staff registration fails. This
  endpoint has no `[Authorize]` at all (it can't — there's no token to
  check); it validates the header manually instead, and is never routed
  through the gateway or exposed to the frontend.
- **Fields `RegisterDto` doesn't collect** — `EmployeeCode`,
  `Designation`, `EmploymentType`, `JoiningDate`, `Salary` — get sensible
  placeholders inside `StaffService.CreateStaffFromAuthAsync`
  (`EmployeeCode` derived from the `StaffId`, `Designation` set to the
  role name, `EmploymentType` defaults to `"Full-Time"`, `Salary` starts
  at `0`). HR edits these to real values afterward via the existing
  `PUT /api/staffs/{id}` — nothing here blocks that normal flow.

**New config, one per service, both must match:**

```env
# AuthService/HMS.AuthService.Api/.env and StaffService/HMS.StaffService.WebApi/.env
INTERNAL_API_KEY=<same long random value in both files>
```

```json
// AuthService/HMS.AuthService.Api/appsettings.json — not a secret, so it's here, not in .env
"StaffService": { "BaseUrl": "http://localhost:5125" }
```

`http://`, not `https://`, on purpose: this is a direct service-to-service
call on localhost with its own header-based auth, not a browser request —
using the http port sidesteps dev-certificate trust entirely for a call
that was never going through `UseHttpsRedirection` anyway (already removed
from every service, see the merge notes above).

### Testing this specifically

1. Set the **same** `INTERNAL_API_KEY` in both `.env` files.
2. Register a `Staff` user with a real `departmentId` for an existing
   hotel/department. Expect 200, and a token with a `staffId` claim.
3. Confirm the matching row exists: `GET /api/staffs/{staffId}` (as `HR`)
   should return it, with a generated `EMP-XXXXXXXX` employee code.
4. **Prove the rollback works**: temporarily set a wrong
   `INTERNAL_API_KEY` in Auth's `.env` only, restart Auth, try registering
   another `Staff` user — expect a 400 ("Staff Service rejected..."), and
   confirm via `POST /api/auth/login` with those exact credentials that
   **no** Identity user was left behind (login should fail with "Invalid
   email or password", not succeed with a token for a half-created user).
   Put the correct key back afterward.
5. Register a `HotelOwner` and confirm **no** call to StaffService happens
   (check StaffService's logs — nothing should be logged for that
   registration) and `GET /api/staffs/{thatStaffId}` returns 404, since
   `HotelOwner` intentionally gets no `Staff` row.
