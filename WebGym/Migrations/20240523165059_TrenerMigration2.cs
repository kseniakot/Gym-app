using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebGym.Migrations
{
    /// <inheritdoc />
    public partial class TrenerMigration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "WorkHours",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "WorkDays",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "WorkHours");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "WorkDays");
        }
    }
}
