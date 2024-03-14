using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAssistant.Infrastructure.Migrations
{
    public partial class seed_products : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEO1Cvf3NVuu9IyZds37Yx3m1i/4m2fWv+5xHcVMnND+xWSs7biyZhVuaRrgk2aAuEA==", "cd92f982-f417-47bf-90b7-f9a04b644ff1" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "AddedOn", "CategoryId", "Count", "Name", "UserId", "Weight" },
                values: new object[] { 1, new DateTime(2024, 3, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 3, "Steak", null, null });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "AddedOn", "CategoryId", "Count", "Name", "UserId", "Weight" },
                values: new object[] { 2, new DateTime(2024, 3, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 10, "Apple", null, null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEBBITStPTVBoPYKox8Z5S+xCn7EKq+wCRsEEdoyvlMKww9Dh5POj4NYtBv1Im8lErQ==", "9ea857f1-8abc-4719-a333-b4291066884d" });
        }
    }
}
