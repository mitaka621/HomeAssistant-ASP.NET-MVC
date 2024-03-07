using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAssistant.Infrastructure.Migrations
{
    public partial class seed_Admin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "7e2d4805-c978-4600-9663-a9cafa2a54be", "7e2d4805-c978-4600-9663-a9cafa2a54be", "NormalUser", null });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "f23e50cd-3de0-4420-ae9b-6ce529f3128f", "f23e50cd-3de0-4420-ae9b-6ce529f3128f", "Admin", null });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedOn", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "e2246145-9dd8-4902-ae41-68096b5ca738", 0, "e2246145-9dd8-4902-ae41-68096b5ca738", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin", false, "", "", false, null, "admin", "admin", "AQAAAAEAACcQAAAAEP05RwICBZiUUXYho5eZVvAMg13mQuMlVAvfnVkFvc3wndwP1fVZhTHJ6V8jJDD5RA==", null, false, "f35d1659-9b27-4e92-b9d6-393bbd49484b", false, "admin" });

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
                keyValue: "7e2d4805-c978-4600-9663-a9cafa2a54be");

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

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "AspNetUsers");
        }
    }
}
