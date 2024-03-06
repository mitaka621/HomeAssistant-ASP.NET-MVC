using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAssistant.Infrastructure.Migrations
{
    public partial class seed_adminUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "41df8142-5af6-46c7-b88f-bcfb45eaf634", "ef26971d-5b5c-4568-9e48-a4dff9ecf4d2", "NormalUser", null });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "f23e50cd-3de0-4420-ae9b-6ce529f3128f", "da16e754-7e40-476e-8875-c8994e9cc476", "Admin", null });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "e2246145-9dd8-4902-ae41-68096b5ca738", 0, "8fa69e17-ad73-4839-8a2b-b6865a8a5c7d", "admin", false, "", "", false, null, "admin", "admin", "AQAAAAEAACcQAAAAEL5FvthfLwEM7M4/cw6F2JJooCYf4w1tdIu/Mw0YsyDdkD5ZGjGWrzd3gUirlXGgOw==", null, false, "692597fb-6560-4177-8d99-937b769ea575", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "f23e50cd-3de0-4420-ae9b-6ce529f3128f", "e2246145-9dd8-4902-ae41-68096b5ca738" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "41df8142-5af6-46c7-b88f-bcfb45eaf634");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "f23e50cd-3de0-4420-ae9b-6ce529f3128f", "e2246145-9dd8-4902-ae41-68096b5ca738" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f23e50cd-3de0-4420-ae9b-6ce529f3128f");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738");
        }
    }
}
