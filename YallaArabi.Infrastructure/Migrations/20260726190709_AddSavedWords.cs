using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YallaArabi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSavedWords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "saved_words",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    WordId = table.Column<Guid>(type: "uuid", nullable: false),
                    SavedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_saved_words", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_saved_words_UserId_WordId",
                table: "saved_words",
                columns: new[] { "UserId", "WordId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "saved_words");
        }
    }
}
