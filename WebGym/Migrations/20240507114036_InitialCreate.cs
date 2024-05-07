using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebGym.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsBanned = table.Column<bool>(type: "boolean", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActiveFreezes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DaysLeft = table.Column<int>(type: "integer", nullable: false),
                    MembershipInstanceId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveFreezes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FreezeInstances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MemberId = table.Column<int>(type: "integer", nullable: false),
                    FreezeId = table.Column<int>(type: "integer", nullable: false),
                    MembershipInstanceId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FreezeInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FreezeInstances_Users_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Memberships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Months = table.Column<int>(type: "integer", nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: true),
                    FreezeId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Memberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Memberships_FreezeInstances_FreezeId",
                        column: x => x.FreezeId,
                        principalTable: "FreezeInstances",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Freezes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Days = table.Column<int>(type: "integer", nullable: false),
                    MinDays = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    MembershipId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Freezes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Freezes_Memberships_MembershipId",
                        column: x => x.MembershipId,
                        principalTable: "Memberships",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MembershipInstances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MembershipId = table.Column<int>(type: "integer", nullable: false),
                    MemberId = table.Column<int>(type: "integer", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembershipInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MembershipInstances_Memberships_MembershipId",
                        column: x => x.MembershipId,
                        principalTable: "Memberships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MembershipInstances_Users_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActiveFreezes_MembershipInstanceId",
                table: "ActiveFreezes",
                column: "MembershipInstanceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FreezeInstances_FreezeId",
                table: "FreezeInstances",
                column: "FreezeId");

            migrationBuilder.CreateIndex(
                name: "IX_FreezeInstances_MemberId",
                table: "FreezeInstances",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_FreezeInstances_MembershipInstanceId",
                table: "FreezeInstances",
                column: "MembershipInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Freezes_MembershipId",
                table: "Freezes",
                column: "MembershipId");

            migrationBuilder.CreateIndex(
                name: "IX_MembershipInstances_MemberId",
                table: "MembershipInstances",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MembershipInstances_MembershipId",
                table: "MembershipInstances",
                column: "MembershipId");

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_FreezeId",
                table: "Memberships",
                column: "FreezeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActiveFreezes_MembershipInstances_MembershipInstanceId",
                table: "ActiveFreezes",
                column: "MembershipInstanceId",
                principalTable: "MembershipInstances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FreezeInstances_Freezes_FreezeId",
                table: "FreezeInstances",
                column: "FreezeId",
                principalTable: "Freezes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FreezeInstances_MembershipInstances_MembershipInstanceId",
                table: "FreezeInstances",
                column: "MembershipInstanceId",
                principalTable: "MembershipInstances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FreezeInstances_MembershipInstances_MembershipInstanceId",
                table: "FreezeInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_FreezeInstances_Freezes_FreezeId",
                table: "FreezeInstances");

            migrationBuilder.DropTable(
                name: "ActiveFreezes");

            migrationBuilder.DropTable(
                name: "MembershipInstances");

            migrationBuilder.DropTable(
                name: "Freezes");

            migrationBuilder.DropTable(
                name: "Memberships");

            migrationBuilder.DropTable(
                name: "FreezeInstances");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
