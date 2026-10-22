using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YallaArabi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmailVerificationAttempts",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EmailVerificationCodeHash",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailVerificationExpiresAt",
                table: "users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailVerifiedAt",
                table: "users",
                type: "timestamp with time zone",
                nullable: true);
            // Accounts that existed before verification was added are trusted as they are.
            migrationBuilder.Sql("UPDATE users SET \"EmailVerifiedAt\" = \"CreatedAt\";");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailVerificationAttempts",
                table: "users");

            migrationBuilder.DropColumn(
                name: "EmailVerificationCodeHash",
                table: "users");

            migrationBuilder.DropColumn(
                name: "EmailVerificationExpiresAt",
                table: "users");

            migrationBuilder.DropColumn(
                name: "EmailVerifiedAt",
                table: "users");
        }
    }
}
