using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YallaArabi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoriesLessonsProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LevelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "lessons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    PointsReward = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lessons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "student_lesson_progress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LessonId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_lesson_progress", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_categories_LevelId",
                table: "categories",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_lessons_CategoryId",
                table: "lessons",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_student_lesson_progress_UserId_LessonId",
                table: "student_lesson_progress",
                columns: new[] { "UserId", "LessonId" },
                unique: true);

            migrationBuilder.InsertData(
                table: "categories",
                columns: new[] { "Id", "LevelId", "Name", "OrderIndex" },
                values: new object[,]
                {
                    { "aaaaaaaa-0001-0001-0001-000000000001", "11111111-1111-1111-1111-111111111111", "Greetings", 0 },
                    { "aaaaaaaa-0001-0001-0001-000000000002", "11111111-1111-1111-1111-111111111111", "Numbers", 1 }
                });

            migrationBuilder.InsertData(
                table: "lessons",
                columns: new[] { "Id", "CategoryId", "Title", "OrderIndex", "PointsReward" },
                values: new object[,]
                {
                    { "bbbbbbbb-0001-0001-0001-000000000001", "aaaaaaaa-0001-0001-0001-000000000001", "Hello & Goodbye", 0, 10 },
                    { "bbbbbbbb-0001-0001-0001-000000000002", "aaaaaaaa-0001-0001-0001-000000000001", "How Are You?", 1, 10 },
                    { "bbbbbbbb-0001-0001-0001-000000000003", "aaaaaaaa-0001-0001-0001-000000000002", "Numbers 1-10", 0, 10 },
                    { "bbbbbbbb-0001-0001-0001-000000000004", "aaaaaaaa-0001-0001-0001-000000000002", "Numbers 11-20", 1, 10 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "lessons",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    "bbbbbbbb-0001-0001-0001-000000000001",
                    "bbbbbbbb-0001-0001-0001-000000000002",
                    "bbbbbbbb-0001-0001-0001-000000000003",
                    "bbbbbbbb-0001-0001-0001-000000000004"
                });

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    "aaaaaaaa-0001-0001-0001-000000000001",
                    "aaaaaaaa-0001-0001-0001-000000000002"
                });

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "lessons");

            migrationBuilder.DropTable(
                name: "student_lesson_progress");
        }
    }
}