using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAssistant.Infrastructure.Migrations
{
    public partial class added_ip_for_HomeAssistantUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientIpAddress",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "ClientIpAddress", "PasswordHash", "SecurityStamp" },
                values: new object[] { "", "AQAAAAEAACcQAAAAEAjAZN0Ujw/pu/F7k/+9iPFE2bFu/wdRjnpqtZz+xPy6eLcZD3vng6zPK7TQq6069g==", "94c14e01-6f4a-4b67-9dbc-fb79a7624deb" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientIpAddress",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEOq/sU+MfuR3RqHsuVJ1U2M9iZZsJgk+dQoQCbsIFB0CQLZ0XHj2eSO2BkmuSYILtw==", "854963e2-a0ce-460f-93b3-a6af81e23ef5" });
        }
    }
}
