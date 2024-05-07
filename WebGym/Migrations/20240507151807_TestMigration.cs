using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebGym.Migrations
{
    /// <inheritdoc />
    public partial class TestMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_FreezeInstances_FreezeId",
                table: "Memberships");

            migrationBuilder.DropIndex(
                name: "IX_Memberships_FreezeId",
                table: "Memberships");

            migrationBuilder.DropIndex(
                name: "IX_Freezes_MembershipId",
                table: "Freezes");

            migrationBuilder.DropColumn(
                name: "FreezeId",
                table: "Memberships");

            migrationBuilder.CreateIndex(
                name: "IX_Freezes_MembershipId",
                table: "Freezes",
                column: "MembershipId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Freezes_MembershipId",
                table: "Freezes");

            migrationBuilder.AddColumn<int>(
                name: "FreezeId",
                table: "Memberships",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_FreezeId",
                table: "Memberships",
                column: "FreezeId");

            migrationBuilder.CreateIndex(
                name: "IX_Freezes_MembershipId",
                table: "Freezes",
                column: "MembershipId");

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_FreezeInstances_FreezeId",
                table: "Memberships",
                column: "FreezeId",
                principalTable: "FreezeInstances",
                principalColumn: "Id");
        }
    }
}
