# HMS Backend

.NET 10 microservices behind a YARP gateway, one shared PostgreSQL database, JWT shared across services.

| Service | Folder | Gateway paths |
| --- | --- | --- |
| Gateway | HMS.ApiGateway | https://localhost:7006 |
| Auth | AuthService | /api/auth |
| Tenant | TenantService | /api/tenants |
| Hotel | HotelService | /api/hotels, /api/amenities, /api/hotelamenities |
| Rooms | RoomManagementService | /api/roomtypes, /api/rooms, /api/rateplans, /api/roomtypeamenities |
| Staff | StaffService | /api/staffs, /api/departments, /api/designations, /api/staffnotes, /api/staffdocuments |
| Attendance | AttendanceService | /api/attendances, /api/attendanceanomalies, /api/leaveapplications, /api/shifts |
| Guest | GuestService | /api/guests, /api/bookings, /api/reservations |
| Billing | BillingService | /api/invoices, /api/payments |
| Operations | OperationService | /api/housekeeping, /api/maintenance, /api/servicerequests, /api/roominspections, /api/lostandfound, /api/stafftaskassignments |

## Account hierarchy

SuperAdmin creates TenantAdmin and HotelOwner accounts. TenantAdmin creates HotelOwner accounts in its own tenant. HotelOwner creates HR, Manager and Staff. HR creates Manager and Staff. Registration requires a signed-in creator.
