using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YallaArabi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDailyGoalAndMotivation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DailyGoalMinutes",
                table: "users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LearningMotivation",
                table: "users",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailyGoalMinutes",
                table: "users");

            migrationBuilder.DropColumn(
                name: "LearningMotivation",
                table: "users");
        }
    }
}
