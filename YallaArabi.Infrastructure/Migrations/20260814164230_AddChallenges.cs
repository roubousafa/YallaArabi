using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YallaArabi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChallenges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "challenge_questions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChallengeId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionText = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    OptionA = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OptionB = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OptionC = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    OptionD = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CorrectOption = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_challenge_questions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "challenges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    LevelId = table.Column<Guid>(type: "uuid", nullable: true),
                    PointsReward = table.Column<int>(type: "integer", nullable: false),
                    TimeLimitSeconds = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_challenges", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_challenge_questions_ChallengeId",
                table: "challenge_questions",
                column: "ChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_challenges_LevelId",
                table: "challenges",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_challenges_Status",
                table: "challenges",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "challenge_questions");

            migrationBuilder.DropTable(
                name: "challenges");
        }
    }
}
