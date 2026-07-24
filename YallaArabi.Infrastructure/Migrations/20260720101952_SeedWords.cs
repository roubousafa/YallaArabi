using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YallaArabi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedWords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "words",
                columns: new[]
                {
                    "Id", "LevelId", "ArabicText", "Transliteration", "EnglishMeaning",
                    "NormalizedArabic", "NormalizedTransliteration", "NormalizedEnglish",
                    "AudioUrl", "ImageUrl", "CreatedAt"
                },
                values: new object[,]
                {
                    {
                        "cccccccc-0001-0001-0001-000000000001", "11111111-1111-1111-1111-111111111111",
                        "مرحبا", "mar7aba", "Hello",
                        "مرحبا", "marhaba", "hello",
                        null, null, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    },
                    {
                        "cccccccc-0001-0001-0001-000000000002", "11111111-1111-1111-1111-111111111111",
                        "شكراً", "shukran", "Thank you",
                        "شكرا", "shukran", "thank you",
                        null, null, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    },
                    {
                        "cccccccc-0001-0001-0001-000000000003", "11111111-1111-1111-1111-111111111111",
                        "صباح الخير", "9aba7 el 5air", "Good morning",
                        "صباح الخير", "sabah el khair", "good morning",
                        null, null, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    },
                    {
                        "cccccccc-0001-0001-0001-000000000004", "11111111-1111-1111-1111-111111111111",
                        "كيفك", "keefak", "How are you",
                        "كيفك", "keefak", "how are you",
                        null, null, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    },
                       {
                        "cccccccc-0001-0001-0001-000000000005", "11111111-1111-1111-1111-111111111111",
                        "يعطيك العافية", "ya3teek el 3afye", "Well done / bless you",
                        "يعطيك العافيه", "yaateek el aafye", "well done bless you",
                        null, null, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "words",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    "cccccccc-0001-0001-0001-000000000001",
                    "cccccccc-0001-0001-0001-000000000002",
                    "cccccccc-0001-0001-0001-000000000003",
                    "cccccccc-0001-0001-0001-000000000004",
                    "cccccccc-0001-0001-0001-000000000005"
                });
        }
    }
}