using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAssistant.Infrastructure.Migrations
{
    public partial class Changed_role_name_from_NormalUser_to_StandardUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7e2d4805-c978-4600-9663-a9cafa2a54be",
                columns: new[] { "Name", "NormalizedName" },
                values: new object[] { "StandardUser", "StandardUser" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEDv+rDRWIbxzY+zFSMOQFad9u/nYz5Mxra/RmVRlyg0rtVVj7VCR7D80JKmE70F9xA==", "c0d7d2d2-527d-426c-ba11-cdbdf90a2eae" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7e2d4805-c978-4600-9663-a9cafa2a54be",
                columns: new[] { "Name", "NormalizedName" },
                values: new object[] { "NormalUser", "NormalUser" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEOIZQxDPI1uiWmFcn18kJr7apOaxKjN3x42XTXwLeZApnANn+1G8Yixm+zlceNkXhw==", "d7d4471b-2bc8-4a66-a54d-fbb70d4714d3" });
        }
    }
}
