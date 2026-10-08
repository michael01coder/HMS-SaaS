using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HMS.StaffService.Domain.Migrations
{
    /// <inheritdoc />
    public partial class InitialIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Note",
                table: "StaffNotes",
                newName: "NoteText");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NoteText",
                table: "StaffNotes",
                newName: "Note");
        }
    }
}
