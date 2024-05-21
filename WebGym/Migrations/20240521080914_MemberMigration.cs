using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebGym.Migrations
{
    /// <inheritdoc />
    public partial class MemberMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FreezeInstances_Freezes_FreezeId",
                table: "FreezeInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_FreezeInstances_MembershipInstances_MembershipInstanceId",
                table: "FreezeInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_FreezeInstances_Users_UserId",
                table: "FreezeInstances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FreezeInstances",
                table: "FreezeInstances");

            migrationBuilder.RenameTable(
                name: "FreezeInstances",
                newName: "FreezeInstance");

            migrationBuilder.RenameIndex(
                name: "IX_FreezeInstances_UserId",
                table: "FreezeInstance",
                newName: "IX_FreezeInstance_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_FreezeInstances_MembershipInstanceId",
                table: "FreezeInstance",
                newName: "IX_FreezeInstance_MembershipInstanceId");

            migrationBuilder.RenameIndex(
                name: "IX_FreezeInstances_FreezeId",
                table: "FreezeInstance",
                newName: "IX_FreezeInstance_FreezeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FreezeInstance",
                table: "FreezeInstance",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FreezeInstance_Freezes_FreezeId",
                table: "FreezeInstance",
                column: "FreezeId",
                principalTable: "Freezes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FreezeInstance_MembershipInstances_MembershipInstanceId",
                table: "FreezeInstance",
                column: "MembershipInstanceId",
                principalTable: "MembershipInstances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FreezeInstance_Users_UserId",
                table: "FreezeInstance",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FreezeInstance_Freezes_FreezeId",
                table: "FreezeInstance");

            migrationBuilder.DropForeignKey(
                name: "FK_FreezeInstance_MembershipInstances_MembershipInstanceId",
                table: "FreezeInstance");

            migrationBuilder.DropForeignKey(
                name: "FK_FreezeInstance_Users_UserId",
                table: "FreezeInstance");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FreezeInstance",
                table: "FreezeInstance");

            migrationBuilder.RenameTable(
                name: "FreezeInstance",
                newName: "FreezeInstances");

            migrationBuilder.RenameIndex(
                name: "IX_FreezeInstance_UserId",
                table: "FreezeInstances",
                newName: "IX_FreezeInstances_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_FreezeInstance_MembershipInstanceId",
                table: "FreezeInstances",
                newName: "IX_FreezeInstances_MembershipInstanceId");

            migrationBuilder.RenameIndex(
                name: "IX_FreezeInstance_FreezeId",
                table: "FreezeInstances",
                newName: "IX_FreezeInstances_FreezeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FreezeInstances",
                table: "FreezeInstances",
                column: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_FreezeInstances_Users_UserId",
                table: "FreezeInstances",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
