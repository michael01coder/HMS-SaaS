# HMS-Saas-Backend — merged solution

This is the merged codebase: your YARP gateway + Postgres services, combined
with your friend's ASP.NET Identity-based Auth and Staff services, on one
shared database. Below is exactly what changed, how to run it, and what
still needs a decision from the two of you before it's fully wired.

## What changed in this merge

- **Gateway:** kept your `HMS.ApiGateway` (YARP). Deleted `OcelotGateway`
  entirely — pick one gateway, and YARP was already further along.
- **Auth:** kept your friend's ASP.NET Identity–based `AuthService`
  (`HMS.AuthService.*` projects — Users/Roles/RefreshTokens via
  `IdentityDbContext`). Deleted the old custom-JWT `AuthService.Api` and its
  supporting `AuthService.DTO/Data/Domain/Repository/Service` projects —
  you can't run two auth systems issuing tokens with different claim shapes
  against the same gateway.
- **Database:** every service's `DB_CONNECTION_STRING` now points at the same
  physical database (`HMSDataBase`). Each service keeps its own
  `DbContext` and only touches its own tables — but since they now share one
  physical DB, each `AddDbContext(...)` call was given its own
  `MigrationsHistoryTable(...)` name (e.g. `__EFMigrationsHistory_Tenant`) so
  the five services' migration histories don't collide in the same database.
- **Secrets:** every `appsettings.json` had its plaintext `Jwt:SecretKey` and
  DB password removed. All five services now load `.env` at startup
  (`DotNetEnv.Env.Load()`) and read `JWT_SECRET_KEY` / `DB_CONNECTION_STRING`
  from there first, falling back to `appsettings.json` only for the
  non-secret bits (`Jwt:Issuer`, `Jwt:Audience`, `Jwt:ExpiryInMinutes`).
  `.env` files are git-ignored; `.env.example` at the repo root documents the
  two variables every service needs.
- **Issuer/Audience alignment:** every service now validates tokens against
  `Jwt:Issuer = "HMS.AuthService.Api"` and `Jwt:Audience = "HMS.Microservices"`
  — these are what your friend's `JwtTokenService` actually issues. Your
  three services previously expected `"AuthService.Api"` / `"HotelService"`,
  which would have rejected every real token.
- **Redirect fix, applied everywhere:** removed `app.UseHttpsRedirection()`
  from all five services' `Program.cs`. You hit this bug three times already
  (Postman, then the gateway, then the gateway→service hop) — a downstream
  service redirecting http→https strips the `Authorization` header, and
  since every service now sits behind the gateway rather than being hit
  directly, there's no reason for any of them to redirect on their own.
- **Gateway routes:** added `staff-route` (`/api/staffs`, `/api/departments`,
  `/api/staffnotes`, `/api/staffdocuments` → `https://localhost:7104`),
  widened `tenant-route` to also cover `/api/subscriptionplans` and
  `/api/subscriptions`, and pointed `auth-cluster` at the Identity service's
  real port (`7170`, not the old `7183`).
- **Stray files removed:** a duplicate top-level `HMSTenant.Data`/
  `HMSTenant.Domain` (empty scaffolds, unrelated to the real ones under
  `TenantService/`), an unused default `Class1.cs` class-library project, all
  `bin/`/`obj/`/`.vs/` folders, and `.csproj.user`/`.http` files.
- **Solution file:** `HMS-Saas-Backend.slnx` was rewritten from scratch to
  reference the 28 projects that actually exist on disk now — the one in
  your upload still listed the deleted `AuthService2`/`OcelotGateway`
  projects and a broken path for the kept `AuthService` folder.

## Running it

1. Install PostgreSQL locally if you haven't, and create one database:
   ```sql
   CREATE DATABASE "HMSDataBase";
   ```
2. For **each** of these five folders, copy `.env.example` (at the repo
   root) to a file named `.env` **inside that folder**, then fill in a real
   Postgres password (the same one for all five, since it's one database)
   and one shared `JWT_SECRET_KEY` (generate one, e.g.
   `openssl rand -base64 32`, and paste the same value into all five):
   - `AuthService/HMS.AuthService.Api/.env`
   - `TenantService/HMSTenantService.Api/.env`
   - `HotelService/HMSHotelServices.Api/.env`
   - `RoomManagementService/RoomManagementService.Api/.env`
   - `StaffService/HMS.StaffService.WebApi/.env`

   (Placeholder `.env` files with matching dummy values already exist in
   each folder so the solution opens and restores cleanly — replace the
   password and rotate the JWT key before you push real data into it.)
3. **Generate migrations for Auth and Staff — these didn't exist yet.**
   I can't run `dotnet ef` here (no .NET SDK in this environment), so this
   is the one manual step required before anything will boot:
   ```bash
   cd AuthService/HMS.AuthService.Api
   dotnet ef migrations add InitialIdentity --project ../HMS.AuthService.Domain --startup-project .
   dotnet ef database update --project ../HMS.AuthService.Domain --startup-project .

   cd ../../StaffService/HMS.StaffService.WebApi
   dotnet ef migrations add InitialStaff --project ../HMS.StaffService.Domain --startup-project .
   dotnet ef database update --project ../HMS.StaffService.Domain --startup-project .
   ```
   Tenant, Hotel, and Room Management already have migrations from before —
   those apply as-is against the shared DB:
   ```bash
   cd TenantService/HMSTenantService.Api && dotnet ef database update --project ../HMSTenant.Data --startup-project .
   cd ../../HotelService/HMSHotelServices.Api && dotnet ef database update --project ../HMSHotel.Data --startup-project .
   cd ../../RoomManagementService/RoomManagementService.Api && dotnet ef database update --project ../HMSRoomManagement.Data --startup-project .
   ```
4. Run all six projects (five services + gateway) — via Visual Studio's
   multiple-startup-projects, or five terminals with `dotnet run`, plus the
   gateway. Ports (unchanged from before):

   | Service | HTTPS port | Gateway path prefix |
   |---|---|---|
   | HMS.ApiGateway | — (this is what the frontend calls) | — |
   | AuthService (Identity) | 7170 | `/api/auth` |
   | TenantService | 7225 | `/api/tenants`, `/api/subscriptionplans`, `/api/subscriptions` |
   | HotelService | 7136 | `/api/hotels`, `/api/amenities`, `/api/hotelamenities` |
   | RoomManagementService | 7256 | `/api/roomtypes`, `/api/rooms`, `/api/rateplans`, `/api/roomtypeamenities` |
   | StaffService | 7104 | `/api/staffs`, `/api/departments`, `/api/staffnotes`, `/api/staffdocuments` |

## Decisions still needed from the two of you (not guessed at here)

I deliberately didn't touch these — they're product/logic calls, not
plumbing, and guessing wrong here is worse than leaving it visible:

1. **Role model mismatch — the big one.** Your friend's Identity service only
   ever creates `HR` / `Manager` / `Staff` roles (that's the entire
   `UserRole` enum, and it's what `StaffService`'s controllers check). Your
   `TenantService`/`HotelService`/`RoomManagementService` controllers check
   for `SuperAdmin`, `TenantAdmin`, `HotelOwner`, `HR`, `FrontDeskStaff` —
   roles that Identity's `RegisterAsync` has no path to ever create. Right
   now, nobody can ever hold those four roles, so every endpoint gated to
   them is effectively unreachable. Pick one:
   - Extend `HMS.AuthService.Common.Enums.UserRole` to the full five-role
     set and update `RegisterDto`/`AuthService.RegisterAsync` accordingly, or
   - Change the `Authorize(Roles = ...)` checks in your three services (and
     the frontend's `AppRole` type) down to `HR`/`Manager`/`Staff`.
   Either is a small mechanical change once you decide — I didn't pick for
   you since it's a five-role vs. three-role product decision.
2. **Cross-service staff lookup.** Since everything's one physical database
   now, you have two real options instead of just the HTTP-call one I
   mentioned earlier: (a) still call `StaffService`'s API over HTTP — keeps
   service boundaries clean, works even if you later split the DB apart
   again; or (b) add a small read-only, keyless EF entity in whichever
   service needs staff names, mapped to Staff's `Staff` table directly,
   since it's already in the same DB. (b) is less code short-term but
   quietly re-creates the coupling a shared DB is supposed to avoid — your
   call.
3. **`RegisterDto` fields that go nowhere.** `RegisterDto` collects
   `FirstName`, `LastName`, `Phone`, and `DepartmentId`, but
   `AuthService.RegisterAsync` never persists them anywhere — it only ever
   writes `Email`/`HotelId`/`StaffId` onto `ApplicationUser`. `StaffId` is a
   freshly generated GUID with no matching `Staff` row created anywhere.
   You'll want `RegisterAsync` to also call `StaffService` to create the
   matching `Staff` record using that same `StaffId`, or drop those fields
   from `RegisterDto` until that call exists.
4. **Amenities/HotelAmenities controllers** (from an earlier conversation)
   were never actually added to `HotelService` in the project you uploaded
   — only `RoomTypeAmenities` made it into `RoomManagementService`. If you
   still want Hotel-level amenities, that code is in this chat's history
   above; it wasn't re-added here since this pass was scoped to the
   infra/auth/gateway merge specifically.
5. **Frontend (`hms-portal`)** still expects the five-role model
   (`SuperAdmin`/`HotelOwner`/`HotelManager`(renamed `HR`)/`FrontDeskStaff`/
   `Guest`) in `src/types/index.ts`. Whichever way #1 above goes, the
   frontend's `AppRole` union needs to match it exactly, or role-gated pages
   will silently show "Access restricted" for real logged-in users.

## Branching

With this as your shared `main`, a reasonable split for the two of you:
- You: `feature/tenant-service`, `feature/hotel-service`,
  `feature/room-service`, `feature/billing-service`,
  `feature/operations-service`, `feature/gateway`
- Your friend: `feature/auth-service`, `feature/staff-service`,
  `feature/booking-service`

Since every service is its own set of projects with no shared code between
them (by design), merge conflicts between your branches should mostly be
limited to `HMS-Saas-Backend.slnx` (adding new projects) and
`HMS.ApiGateway/appsettings.json` (adding new routes) — both plain text and
easy to resolve by hand.
