using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YallaArabi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLevels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CurrentLevelId",
                table: "users",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "levels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_levels", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_levels_Code",
                table: "levels",
                column: "Code",
                unique: true);
            migrationBuilder.InsertData(
                table: "levels",
                columns: new[] { "Id", "Code", "Name", "OrderIndex" },
                values: new object[,]
                {
                    { "11111111-1111-1111-1111-111111111111", "A0", "Absolute Beginner", 0 },
                    { "22222222-2222-2222-2222-222222222222", "A1", "Beginner", 1 },
                    { "33333333-3333-3333-3333-333333333333", "A2", "Elementary", 2 },
                    { "44444444-4444-4444-4444-444444444444", "A3", "Pre-Intermediate", 3 },
                    { "55555555-5555-5555-5555-555555555555", "A4", "Intermediate", 4 },
                    { "66666666-6666-6666-6666-666666666666", "A5", "Upper-Intermediate", 5 },
                    { "77777777-7777-7777-7777-777777777777", "A6", "Advanced", 6 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "levels",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    "11111111-1111-1111-1111-111111111111",
                    "22222222-2222-2222-2222-222222222222",
                    "33333333-3333-3333-3333-333333333333",
                    "44444444-4444-4444-4444-444444444444",
                    "55555555-5555-5555-5555-555555555555",
                    "66666666-6666-6666-6666-666666666666",
                    "77777777-7777-7777-7777-777777777777"
                });
            migrationBuilder.DropTable(
                name: "levels");

            migrationBuilder.DropColumn(
                name: "CurrentLevelId",
                table: "users");
        }
    }
}
