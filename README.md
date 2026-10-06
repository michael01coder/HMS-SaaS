# HMS-Saas-Backend

Five services + a YARP gateway, one shared database, one role model.
See **ARCHITECTURE.md** for the full explanation of the project structure
and how the role/authorization model works — this file is just setup +
what's where.

## Services

| Folder | Owns | Gateway path prefix | Port |
|---|---|---|---|
| `HMS.ApiGateway` | Routing, single entry point | — | — |
| `AuthService` | Identity, login, JWT, roles | `/api/auth` | 7170 |
| `TenantService` | Tenants | `/api/tenants` | 7225 |
| `HotelService` | Hotels, Amenities, HotelAmenities | `/api/hotels`, `/api/amenities`, `/api/hotelamenities` | 7136 |
| `RoomManagementService` | RoomTypes, Rooms, RatePlans, RoomTypeAmenities | `/api/roomtypes`, `/api/rooms`, `/api/rateplans`, `/api/roomtypeamenities` | 7256 |
| `StaffService` | Staff, Departments, StaffNotes, StaffDocuments | `/api/staffs`, `/api/departments`, etc. | 7104 |

Every service has the same six-project shape (`Common` / `Domain` / `DTO` /
`Repository` / `Service` / `Api`-or-`WebApi`) — see ARCHITECTURE.md §1.

## Running it

1. Create one Postgres database:
   ```sql
   CREATE DATABASE "HMSDataBase";
   ```
2. Each service has its own `.env` (git-ignored) — already present with
   matching placeholder values. Replace the password and generate one real
   shared `JWT_SECRET_KEY` (e.g. `openssl rand -base64 32`), pasted into
   **all five** `.env` files identically:
   - `AuthService/HMS.AuthService.Api/.env`
   - `TenantService/HMSTenantService.Api/.env`
   - `HotelService/HMSHotelServices.Api/.env`
   - `RoomManagementService/RoomManagementService.Api/.env`
   - `StaffService/HMS.StaffService.WebApi/.env`
3. **Run migrations — the `--project` path changed for three services**
   since this pass moved `DbContext` into `Domain` (see ARCHITECTURE.md §3
   for why). Full commands:
   ```bash
   cd AuthService/HMS.AuthService.Api
   dotnet ef migrations add InitialIdentity --project ../HMS.AuthService.Domain --startup-project .
   dotnet ef database update --project ../HMS.AuthService.Domain --startup-project .

   cd ../../StaffService/HMS.StaffService.WebApi
   dotnet ef database update --project ../HMS.StaffService.Domain --startup-project .

   cd ../../TenantService/HMSTenantService.Api
   dotnet ef database update --project ../HMSTenant.Domain --startup-project .

   cd ../../HotelService/HMSHotelServices.Api
   dotnet ef migrations add AddAmenities --project ../HMSHotel.Domain --startup-project .
   dotnet ef database update --project ../HMSHotel.Domain --startup-project .

   cd ../../RoomManagementService/RoomManagementService.Api
   dotnet ef database update --project ../HMSRoomManagement.Domain --startup-project .
   ```
4. Run all five services + the gateway. Test each service directly on its
   own port first, then through the gateway — see ARCHITECTURE.md §4 for
   the specific test sequence (role validation, scoping, the Front Desk
   department check).

## What changed in this pass

- Every service restructured to the same `Common`/`Domain`/`DTO`/
  `Repository`/`Service`/`Api` shape (Staff's own shape — see
  ARCHITECTURE.md §1).
- Role model unified to six roles (`SuperAdmin`, `TenantAdmin`,
  `HotelOwner`, `HR`, `Manager`, `Staff`) with real validation at
  registration time, a `TenantId` added end-to-end, and a
  department-based (not role-based) way to express "Front Desk" — see
  ARCHITECTURE.md §2.
- `Amenities`/`HotelAmenities` controllers (previously only given as
  chat code) are now actually in `HotelService`.
- `ExceptionMiddleware` fixed in all five services (was only mapping
  `NotFound`/`BadRequest`) and registered in Staff's `Program.cs` (existed
  but was never wired in).
- `RefreshToken` re-keyed on `UserId` instead of the now-nullable
  `StaffId`.

## Still open (unchanged from before, flagged again for visibility)

- `RegisterDto` still collects `firstName`/`lastName`/`phone` but
  `ApplicationUser` doesn't persist them, and `RegisterAsync` doesn't yet
  call Staff Service to create the matching `Staff` row for the `StaffId`
  it generates.
- The frontend (`hms-portal`)'s `AppRole` type still needs to be updated
  to this six-role set (it currently has a different five-role list from
  an earlier draft of this design).
