using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebGym.Migrations
{
    /// <inheritdoc />
    public partial class MigrationEndDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "MembershipInstances");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "MembershipInstances",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "MembershipInstances");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "MembershipInstances",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
