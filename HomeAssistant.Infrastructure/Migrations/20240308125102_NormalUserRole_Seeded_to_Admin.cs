using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAssistant.Infrastructure.Migrations
{
    public partial class NormalUserRole_Seeded_to_Admin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "7e2d4805-c978-4600-9663-a9cafa2a54be", "e2246145-9dd8-4902-ae41-68096b5ca738" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEOIZQxDPI1uiWmFcn18kJr7apOaxKjN3x42XTXwLeZApnANn+1G8Yixm+zlceNkXhw==", "d7d4471b-2bc8-4a66-a54d-fbb70d4714d3" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "7e2d4805-c978-4600-9663-a9cafa2a54be", "e2246145-9dd8-4902-ae41-68096b5ca738" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAENw0GYGgFQfadhBO2DWMHbB7vaBhozXFM3HYZIu/2hhd70vuxhP69gFuY5mT5ljG7A==", "f40fa8c1-7398-492a-9660-ce610fbb0a2d" });
        }
    }
}
