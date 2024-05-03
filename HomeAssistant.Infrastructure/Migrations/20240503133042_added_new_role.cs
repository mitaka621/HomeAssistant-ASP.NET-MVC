using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAssistant.Infrastructure.Migrations
{
    public partial class added_new_role : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "af0ff461-d2f4-43e6-a50e-14f73c120761", "af0ff461-d2f4-43e6-a50e-14f73c120761", "NASUser", "NASUSER" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEENtMJtCOI6W5lAVG0evuH2x0wsj4lEh21ovpEbjIkl/M3cktet8uzIpIm8Xoptrww==", "c0006aa5-8b81-4500-8699-e8e01f9a2981" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "af0ff461-d2f4-43e6-a50e-14f73c120761");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEAjAZN0Ujw/pu/F7k/+9iPFE2bFu/wdRjnpqtZz+xPy6eLcZD3vng6zPK7TQq6069g==", "94c14e01-6f4a-4b67-9dbc-fb79a7624deb" });
        }
    }
}
