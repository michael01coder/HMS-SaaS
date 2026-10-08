using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HMS.StaffService.Domain.Migrations
{
    /// <inheritdoc />
    public partial class TableChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AuthorStaffId",
                table: "StaffNotes",
                newName: "CreatedByStaffId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedByStaffId",
                table: "StaffNotes",
                newName: "AuthorStaffId");
        }
    }
}
