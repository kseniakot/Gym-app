using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebGym.Migrations
{
    /// <inheritdoc />
    public partial class NewMigrNotify : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Amount");

            migrationBuilder.DropTable(
                name: "Confirmation");

            migrationBuilder.DropTable(
                name: "Redirection");

            migrationBuilder.AddColumn<string>(
                name: "Amount_Currency",
                table: "Payments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Amount_Id",
                table: "Payments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Amount_Value",
                table: "Payments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Confirmation_Confirmation_url",
                table: "Payments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Confirmation_Id",
                table: "Payments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Confirmation_Type",
                table: "Payments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Amount_Currency",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Amount_Id",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Amount_Value",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Confirmation_Id",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Confirmation_Return_url",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Confirmation_Type",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount_Currency",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Amount_Id",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Amount_Value",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Confirmation_Confirmation_url",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Confirmation_Id",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Confirmation_Type",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Amount_Currency",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Amount_Id",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Amount_Value",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Confirmation_Id",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Confirmation_Return_url",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Confirmation_Type",
                table: "Orders");

            migrationBuilder.CreateTable(
                name: "Amount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Amount_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Confirmation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PaymentId = table.Column<string>(type: "text", nullable: true),
                    Confirmation_url = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Confirmation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Confirmation_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Redirection",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    Return_url = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false)
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
                name: "IX_Amount_OrderId",
                table: "Amount",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Confirmation_PaymentId",
                table: "Confirmation",
                column: "PaymentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Redirection_OrderId",
                table: "Redirection",
                column: "OrderId",
                unique: true);
        }
    }
}
