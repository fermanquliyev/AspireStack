using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspireStack.DbInitializator.Migrations
{
    /// <inheritdoc />
    public partial class UserRoleRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                schema: "UserManagement",
                table: "UserRoles",
                column: "RoleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                schema: "UserManagement",
                table: "UserRoles",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                schema: "UserManagement",
                table: "UserRoles",
                column: "RoleId",
                principalSchema: "UserManagement",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_UserId",
                schema: "UserManagement",
                table: "UserRoles",
                column: "UserId",
                principalSchema: "UserManagement",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                schema: "UserManagement",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UserId",
                schema: "UserManagement",
                table: "UserRoles");

            migrationBuilder.DropIndex(
                name: "IX_UserRoles_RoleId",
                schema: "UserManagement",
                table: "UserRoles");

            migrationBuilder.DropIndex(
                name: "IX_UserRoles_UserId",
                schema: "UserManagement",
                table: "UserRoles");
        }
    }
}
