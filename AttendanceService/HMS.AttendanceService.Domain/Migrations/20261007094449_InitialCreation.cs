using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HMS.AttendanceService.Domain.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Attendances",
                columns: table => new
                {
                    AttendanceId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    HotelId = table.Column<Guid>(type: "uuid", nullable: false),
                    StaffId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttendanceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ClockInTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ClockOutTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TotalHours = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    OvertimeHours = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    ApprovedByStaffId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendances", x => x.AttendanceId);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceAnomalies",
                columns: table => new
                {
                    AnomalyId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttendanceId = table.Column<Guid>(type: "uuid", nullable: false),
                    StaffId = table.Column<Guid>(type: "uuid", nullable: false),
                    AnomalyType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ReportedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceAnomalies", x => x.AnomalyId);
                    table.ForeignKey(
                        name: "FK_AttendanceAnomalies_Attendances_AttendanceId",
                        column: x => x.AttendanceId,
                        principalTable: "Attendances",
                        principalColumn: "AttendanceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceAnomalies_AttendanceId",
                table: "AttendanceAnomalies",
                column: "AttendanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_HotelId_StaffId_AttendanceDate",
                table: "Attendances",
                columns: new[] { "HotelId", "StaffId", "AttendanceDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceAnomalies");

            migrationBuilder.DropTable(
                name: "Attendances");
        }
    }
}
