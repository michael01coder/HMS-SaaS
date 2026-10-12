using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HMS.AttendanceService.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddShiftTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Shifts",
                columns: table => new
                {
                    ShiftId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    HotelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ShiftName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ShiftCategory = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shifts", x => x.ShiftId);
                });

            migrationBuilder.CreateTable(
                name: "ShiftAllocations",
                columns: table => new
                {
                    AllocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    HotelId = table.Column<Guid>(type: "uuid", nullable: false),
                    StaffId = table.Column<Guid>(type: "uuid", nullable: false),
                    ShiftId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkDate = table.Column<DateOnly>(type: "date", nullable: false),
                    AllocationType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftAllocations", x => x.AllocationId);
                    table.ForeignKey(
                        name: "FK_ShiftAllocations_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "ShiftId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShiftSwapRequests",
                columns: table => new
                {
                    SwapRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    HotelId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequesterStaffId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequesterAllocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetStaffId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetAllocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Reason = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ApprovedByStaffId = table.Column<Guid>(type: "uuid", nullable: true),
                    RequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RespondedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftSwapRequests", x => x.SwapRequestId);
                    table.ForeignKey(
                        name: "FK_ShiftSwapRequests_ShiftAllocations_RequesterAllocationId",
                        column: x => x.RequesterAllocationId,
                        principalTable: "ShiftAllocations",
                        principalColumn: "AllocationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShiftSwapRequests_ShiftAllocations_TargetAllocationId",
                        column: x => x.TargetAllocationId,
                        principalTable: "ShiftAllocations",
                        principalColumn: "AllocationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShiftAllocations_ShiftId",
                table: "ShiftAllocations",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftSwapRequests_RequesterAllocationId",
                table: "ShiftSwapRequests",
                column: "RequesterAllocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftSwapRequests_TargetAllocationId",
                table: "ShiftSwapRequests",
                column: "TargetAllocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShiftSwapRequests");

            migrationBuilder.DropTable(
                name: "ShiftAllocations");

            migrationBuilder.DropTable(
                name: "Shifts");
        }
    }
}
