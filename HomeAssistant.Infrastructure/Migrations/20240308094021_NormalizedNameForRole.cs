using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAssistant.Infrastructure.Migrations
{
    public partial class NormalizedNameForRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7e2d4805-c978-4600-9663-a9cafa2a54be",
                column: "NormalizedName",
                value: "NormalUser");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f23e50cd-3de0-4420-ae9b-6ce529f3128f",
                column: "NormalizedName",
                value: "Admin");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAELozWXVCFPiMGre6PZINoqUp26c0SKhvyn1ZuHf4b3cqk4qBzcLP8hXvoAtdwmy/XA==", "9278c0b7-2454-4d18-8560-adda302e8a37" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7e2d4805-c978-4600-9663-a9cafa2a54be",
                column: "NormalizedName",
                value: null);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f23e50cd-3de0-4420-ae9b-6ce529f3128f",
                column: "NormalizedName",
                value: null);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEP05RwICBZiUUXYho5eZVvAMg13mQuMlVAvfnVkFvc3wndwP1fVZhTHJ6V8jJDD5RA==", "f35d1659-9b27-4e92-b9d6-393bbd49484b" });
        }
    }
}
