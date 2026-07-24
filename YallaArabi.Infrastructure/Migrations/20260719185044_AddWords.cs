using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YallaArabi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "words",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LevelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ArabicText = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Transliteration = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EnglishMeaning = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    NormalizedArabic = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NormalizedTransliteration = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NormalizedEnglish = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    AudioUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_words", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_words_LevelId",
                table: "words",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_words_NormalizedArabic",
                table: "words",
                column: "NormalizedArabic");

            migrationBuilder.CreateIndex(
                name: "IX_words_NormalizedEnglish",
                table: "words",
                column: "NormalizedEnglish");

            migrationBuilder.CreateIndex(
                name: "IX_words_NormalizedTransliteration",
                table: "words",
                column: "NormalizedTransliteration");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "words");
        }
    }
}
