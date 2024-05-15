using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebGym.Migrations
{
    /// <inheritdoc />
    public partial class MigrationGetPayment107 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Confirmation_Orders_OrderId",
                table: "Confirmation");

            migrationBuilder.DropIndex(
                name: "IX_Confirmation_OrderId",
                table: "Confirmation");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Confirmation");

            migrationBuilder.RenameColumn(
                name: "ConfirmationUrl",
                table: "Confirmation",
                newName: "Confirmation_url");

            migrationBuilder.CreateTable(
                name: "Redirection",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Return_url = table.Column<string>(type: "text", nullable: false),
                    OrderId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Redirection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Redirection_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Redirection_OrderId",
                table: "Redirection",
                column: "OrderId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Redirection");

            migrationBuilder.RenameColumn(
                name: "Confirmation_url",
                table: "Confirmation",
                newName: "ConfirmationUrl");

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "Confirmation",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Confirmation_OrderId",
                table: "Confirmation",
                column: "OrderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Confirmation_Orders_OrderId",
                table: "Confirmation",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
