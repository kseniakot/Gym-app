using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebGym.Migrations
{
    /// <inheritdoc />
    public partial class TestMigration3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Freezes_Memberships_MembershipId",
                table: "Freezes");

            migrationBuilder.DropIndex(
                name: "IX_Freezes_MembershipId",
                table: "Freezes");

            migrationBuilder.DropColumn(
                name: "MembershipId",
                table: "Freezes");

            migrationBuilder.AddColumn<int>(
                name: "FreezeId",
                table: "Memberships",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_FreezeId",
                table: "Memberships",
                column: "FreezeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_Freezes_FreezeId",
                table: "Memberships",
                column: "FreezeId",
                principalTable: "Freezes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_Freezes_FreezeId",
                table: "Memberships");

            migrationBuilder.DropIndex(
                name: "IX_Memberships_FreezeId",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "FreezeId",
                table: "Memberships");

            migrationBuilder.AddColumn<int>(
                name: "MembershipId",
                table: "Freezes",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Freezes_MembershipId",
                table: "Freezes",
                column: "MembershipId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Freezes_Memberships_MembershipId",
                table: "Freezes",
                column: "MembershipId",
                principalTable: "Memberships",
                principalColumn: "Id");
        }
    }
}
