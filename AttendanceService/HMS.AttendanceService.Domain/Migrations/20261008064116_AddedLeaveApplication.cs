using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HMS.AttendanceService.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddedLeaveApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LeaveApplications",
                columns: table => new
                {
                    LeaveApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    HotelId = table.Column<Guid>(type: "uuid", nullable: false),
                    StaffId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeaveTypeName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TotalDays = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AppliedOn = table.Column<DateOnly>(type: "date", nullable: false),
                    ApprovedByStaffId = table.Column<Guid>(type: "uuid", nullable: true),
                    ApprovalDate = table.Column<DateOnly>(type: "date", nullable: true),
                    DefaultDaysPerYear = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveApplications", x => x.LeaveApplicationId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeaveApplications_HotelId_StaffId_StartDate",
                table: "LeaveApplications",
                columns: new[] { "HotelId", "StaffId", "StartDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeaveApplications");
        }
    }
}
