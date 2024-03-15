using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAssistant.Infrastructure.Migrations
{
    public partial class updated_product_fields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Products",
                type: "nvarchar(450)",
                nullable: true,
                comment: "UserId who added the product (optional)",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEPPOt/H3kXgn/N86bLEADQ8q4UwYEPmmzzo76Bb2g49j5dl4mHLQbFMRm8BT+pNSCg==", "94f4a747-3621-4481-aa0c-7a151270fd4b" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Products",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true,
                oldComment: "UserId who added the product (optional)");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEO1Cvf3NVuu9IyZds37Yx3m1i/4m2fWv+5xHcVMnND+xWSs7biyZhVuaRrgk2aAuEA==", "cd92f982-f417-47bf-90b7-f9a04b644ff1" });
        }
    }
}
