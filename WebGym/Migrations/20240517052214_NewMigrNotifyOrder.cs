using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebGym.Migrations
{
    /// <inheritdoc />
    public partial class NewMigrNotifyOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MembershipId",
                table: "Orders",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_MembershipId",
                table: "Orders",
                column: "MembershipId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Memberships_MembershipId",
                table: "Orders",
                column: "MembershipId",
                principalTable: "Memberships",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Memberships_MembershipId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_MembershipId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "MembershipId",
                table: "Orders");
        }
    }
}
