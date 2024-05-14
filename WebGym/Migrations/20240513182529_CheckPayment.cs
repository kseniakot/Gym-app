using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebGym.Migrations
{
    /// <inheritdoc />
    public partial class CheckPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Paid = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Payments_PaymentId1",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Users_PaymentId1",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PaymentId1",
                table: "Users");
        }
    }
}
