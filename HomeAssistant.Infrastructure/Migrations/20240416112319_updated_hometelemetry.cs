using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAssistant.Infrastructure.Migrations
{
    public partial class updated_hometelemetry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Tempreture",
                table: "homeTelemetries",
                newName: "Temperature");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEFNeSp8kPANRsiihmwCjz57l8bphILoIi68h9NuLmyHVhFiLeM0kp/UmYxtsp6Mbig==", "9c041d08-cd4d-451a-ab00-d704b83e693c" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Temperature",
                table: "homeTelemetries",
                newName: "Tempreture");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEEPK2wzcyrekSUPv7ertCwhf6OjdDMjNoeJWRUJZHIqflL3xE9eYLTcXu7Bf/I8vuw==", "70bbead2-249b-4488-8cfc-4922b96c113a" });
        }
    }
}
