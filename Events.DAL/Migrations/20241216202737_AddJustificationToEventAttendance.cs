using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Events.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddJustificationToEventAttendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Justification",
                table: "EventAttendances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Justification",
                table: "EventAttendances");
        }
    }
}
