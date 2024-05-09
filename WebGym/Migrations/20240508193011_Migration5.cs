using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebGym.Migrations
{
    /// <inheritdoc />
    public partial class Migration5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FreezeInstances_Users_MemberId",
                table: "FreezeInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_MembershipInstances_Users_MemberId",
                table: "MembershipInstances");

            migrationBuilder.RenameColumn(
                name: "MemberId",
                table: "MembershipInstances",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_MembershipInstances_MemberId",
                table: "MembershipInstances",
                newName: "IX_MembershipInstances_UserId");

            migrationBuilder.RenameColumn(
                name: "MemberId",
                table: "FreezeInstances",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_FreezeInstances_MemberId",
                table: "FreezeInstances",
                newName: "IX_FreezeInstances_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FreezeInstances_Users_UserId",
                table: "FreezeInstances",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MembershipInstances_Users_UserId",
                table: "MembershipInstances",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FreezeInstances_Users_UserId",
                table: "FreezeInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_MembershipInstances_Users_UserId",
                table: "MembershipInstances");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "MembershipInstances",
                newName: "MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_MembershipInstances_UserId",
                table: "MembershipInstances",
                newName: "IX_MembershipInstances_MemberId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "FreezeInstances",
                newName: "MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_FreezeInstances_UserId",
                table: "FreezeInstances",
                newName: "IX_FreezeInstances_MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_FreezeInstances_Users_MemberId",
                table: "FreezeInstances",
                column: "MemberId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MembershipInstances_Users_MemberId",
                table: "MembershipInstances",
                column: "MemberId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
