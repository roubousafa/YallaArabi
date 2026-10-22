using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YallaArabi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAiMode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ai_dictionary_entries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Query = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NormalizedQuery = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Dialect = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    DialectText = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    EnglishMeaning = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Arabizi = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ImageSource = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ImageCredit = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ImageCreditUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Provider = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    RequestedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    RequestedLocalDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ai_dictionary_entries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ai_mode_settings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Access = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DailyLimit = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ai_mode_settings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ai_dictionary_entries_NormalizedQuery_Dialect",
                table: "ai_dictionary_entries",
                columns: new[] { "NormalizedQuery", "Dialect" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ai_dictionary_entries_RequestedByUserId_RequestedLocalDate",
                table: "ai_dictionary_entries",
                columns: new[] { "RequestedByUserId", "RequestedLocalDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ai_dictionary_entries_Status_CreatedAt",
                table: "ai_dictionary_entries",
                columns: new[] { "Status", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ai_dictionary_entries");

            migrationBuilder.DropTable(
                name: "ai_mode_settings");
        }
    }
}
