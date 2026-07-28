using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YallaArabi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWordReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "word_reviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    WordId = table.Column<Guid>(type: "uuid", nullable: false),
                    RepetitionCount = table.Column<int>(type: "integer", nullable: false),
                    EaseFactor = table.Column<double>(type: "double precision", nullable: false),
                    IntervalDays = table.Column<int>(type: "integer", nullable: false),
                    NextReviewAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_word_reviews", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_word_reviews_UserId_NextReviewAt",
                table: "word_reviews",
                columns: new[] { "UserId", "NextReviewAt" });

            migrationBuilder.CreateIndex(
                name: "IX_word_reviews_UserId_WordId",
                table: "word_reviews",
                columns: new[] { "UserId", "WordId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "word_reviews");
        }
    }
}
