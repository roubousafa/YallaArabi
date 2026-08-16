using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YallaArabi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPointRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "point_rules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ActionKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_point_rules", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_point_rules_ActionKey",
                table: "point_rules",
                column: "ActionKey",
                unique: true);

            migrationBuilder.InsertData(
                table: "point_rules",
                columns: new[] { "Id", "ActionKey", "DisplayName", "Description", "Points", "IsActive", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("dddddddd-0001-0001-0001-000000000001"), "lesson_completed", "Lesson completed", "Points awarded when a student completes a lesson", 10, true, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("dddddddd-0001-0001-0001-000000000002"), "level_completed", "Level completed", "Bonus points when a student completes an entire level", 100, true, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("dddddddd-0001-0001-0001-000000000003"), "word_reviewed_easy", "Word reviewed (Easy)", "Points when a student rates a reviewed word as easy", 3, true, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("dddddddd-0001-0001-0001-000000000004"), "word_reviewed_medium", "Word reviewed (Medium)", "Points when a student rates a reviewed word as medium", 2, true, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("dddddddd-0001-0001-0001-000000000005"), "word_reviewed_hard", "Word reviewed (Hard)", "Points when a student rates a reviewed word as hard", 1, true, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("dddddddd-0001-0001-0001-000000000006"), "word_saved", "Word saved", "Points when a student saves a word to their folder", 1, true, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("dddddddd-0001-0001-0001-000000000007"), "daily_goal_reached", "Daily goal reached", "Points when a student reaches their daily study goal", 15, true, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("dddddddd-0001-0001-0001-000000000008"), "challenge_won", "Challenge won", "Points when a student wins a challenge", 50, true, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("dddddddd-0001-0001-0001-000000000009"), "challenge_participated", "Challenge participated", "Points when a student participates in a challenge", 10, true, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "point_rules");
        }
    }
}
