using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebGym.Migrations
{
    /// <inheritdoc />
    public partial class Migration3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_Freezes_FreezeId",
                table: "Memberships");

            migrationBuilder.AlterColumn<int>(
                name: "FreezeId",
                table: "Memberships",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_Freezes_FreezeId",
                table: "Memberships",
                column: "FreezeId",
                principalTable: "Freezes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_Freezes_FreezeId",
                table: "Memberships");

            migrationBuilder.AlterColumn<int>(
                name: "FreezeId",
                table: "Memberships",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_Freezes_FreezeId",
                table: "Memberships",
                column: "FreezeId",
                principalTable: "Freezes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
