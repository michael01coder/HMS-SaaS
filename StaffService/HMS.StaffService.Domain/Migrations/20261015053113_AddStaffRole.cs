using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HMS.StaffService.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddStaffRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Staff",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Staff");
        }
    }
}
