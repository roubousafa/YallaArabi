using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YallaArabi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPointTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "point_transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActionKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    RelatedEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    Note = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    EarnedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_point_transactions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_point_transactions_UserId_ActionKey_RelatedEntityId",
                table: "point_transactions",
                columns: new[] { "UserId", "ActionKey", "RelatedEntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_point_transactions_UserId_EarnedAt",
                table: "point_transactions",
                columns: new[] { "UserId", "EarnedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "point_transactions");
        }
    }
}
