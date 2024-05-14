using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebGym.Migrations
{
    /// <inheritdoc />
    public partial class Checkpayment10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Payments_PaymentId1",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_PaymentId1",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PaymentId1",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UserId",
                table: "Payments",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Users_UserId",
                table: "Payments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Users_UserId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_UserId",
                table: "Payments");

            migrationBuilder.AddColumn<string>(
                name: "PaymentId",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentId1",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PaymentId1",
                table: "Users",
                column: "PaymentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Payments_PaymentId1",
                table: "Users",
                column: "PaymentId1",
                principalTable: "Payments",
                principalColumn: "Id");
        }
    }
}
