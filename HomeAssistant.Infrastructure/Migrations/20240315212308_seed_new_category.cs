using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAssistant.Infrastructure.Migrations
{
    public partial class seed_new_category : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEGtCnceGiEzItbUw8pST4m14grfcrnHZrFAttMWidwjxMY9je+JVWCMStoeEQTgHAg==", "d108c98a-0885-4413-a6cc-fb8f5779eed1" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[] { 7, "Condiments and Sauces" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEPPOt/H3kXgn/N86bLEADQ8q4UwYEPmmzzo76Bb2g49j5dl4mHLQbFMRm8BT+pNSCg==", "94f4a747-3621-4481-aa0c-7a151270fd4b" });
        }
    }
}
