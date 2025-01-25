using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspireStack.DbInitializator.Migrations
{
    /// <inheritdoc />
    public partial class UserAuditColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                schema: "UserManagement",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                schema: "UserManagement",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                schema: "UserManagement",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "UserManagement",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "EmailVerified",
                schema: "UserManagement",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                schema: "UserManagement",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "UserManagement",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                schema: "UserManagement",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifierId",
                schema: "UserManagement",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                schema: "UserManagement",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                schema: "UserManagement",
                table: "Users",
                type: "text",
                nullable: true,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberVerified",
                schema: "UserManagement",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationTime",
                schema: "UserManagement",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                schema: "UserManagement",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                schema: "UserManagement",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Email",
                schema: "UserManagement",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EmailVerified",
                schema: "UserManagement",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FirstName",
                schema: "UserManagement",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "UserManagement",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                schema: "UserManagement",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastModifierId",
                schema: "UserManagement",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastName",
                schema: "UserManagement",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                schema: "UserManagement",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PhoneNumberVerified",
                schema: "UserManagement",
                table: "Users");
        }
    }
}
