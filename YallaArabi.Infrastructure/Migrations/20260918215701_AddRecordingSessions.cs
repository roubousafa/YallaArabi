using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YallaArabi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRecordingSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "recording_session_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    WordId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderNumber = table.Column<int>(type: "integer", nullable: false),
                    TextToRead = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ClipUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DurationSeconds = table.Column<double>(type: "double precision", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FlagReason = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recording_session_items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "recording_sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Dialect = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Gender = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    VoiceProfileId = table.Column<Guid>(type: "uuid", nullable: true),
                    WordsCount = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UploadedAudioUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ClipsDetected = table.Column<int>(type: "integer", nullable: false),
                    ClipsFlagged = table.Column<int>(type: "integer", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recording_sessions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_recording_session_items_SessionId_OrderNumber",
                table: "recording_session_items",
                columns: new[] { "SessionId", "OrderNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_recording_session_items_Status",
                table: "recording_session_items",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_recording_sessions_Status",
                table: "recording_sessions",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "recording_session_items");

            migrationBuilder.DropTable(
                name: "recording_sessions");
        }
    }
}
