using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAssistant.Infrastructure.Migrations
{
    public partial class added_new_role_WakeOnLan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "eaedfa94-c398-496f-afca-3d4feac3ac82", "eaedfa94-c398-496f-afca-3d4feac3ac82", "WakeOnLanAccess", "WakeOnLanAccess" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEIkDEmwiX74e8c5QrLaV8LFwo33kI2zBKBrEos/jlex5yfzjtAnwuuckTtXp/0/Txw==", "c033902b-f72c-4bf2-a51c-fdd08ccc187d" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "eaedfa94-c398-496f-afca-3d4feac3ac82");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAELoDcJ30OJg3Ndnl3ka3y7SkXHwR3LqajEN7hoT8RH9ENkLxBcQGRTH8+tU+Z/X6jQ==", "88df07f0-44d8-4d0d-b6a5-461ab1b2323d" });
        }
    }
}
