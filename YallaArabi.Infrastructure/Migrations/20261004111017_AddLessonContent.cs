using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YallaArabi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLessonContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "lesson_questions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LessonId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    QuestionText = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    OptionA = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    OptionB = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    OptionC = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    OptionD = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CorrectOption = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    WordId = table.Column<Guid>(type: "uuid", nullable: true),
                    AudioUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lesson_questions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "lesson_words",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LessonId = table.Column<Guid>(type: "uuid", nullable: false),
                    WordId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lesson_words", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_lesson_questions_LessonId",
                table: "lesson_questions",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_lesson_questions_WordId",
                table: "lesson_questions",
                column: "WordId");

            migrationBuilder.CreateIndex(
                name: "IX_lesson_words_LessonId_WordId",
                table: "lesson_words",
                columns: new[] { "LessonId", "WordId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_lesson_words_WordId",
                table: "lesson_words",
                column: "WordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "lesson_questions");

            migrationBuilder.DropTable(
                name: "lesson_words");
        }
    }
}
