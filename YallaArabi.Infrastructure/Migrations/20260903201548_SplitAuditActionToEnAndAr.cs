using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YallaArabi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SplitAuditActionToEnAndAr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Action",
                table: "audit_logs",
                newName: "ActionEn");

            migrationBuilder.AddColumn<string>(
                name: "ActionAr",
                table: "audit_logs",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionAr",
                table: "audit_logs");

            migrationBuilder.RenameColumn(
                name: "ActionEn",
                table: "audit_logs",
                newName: "Action");
        }
    }
}
