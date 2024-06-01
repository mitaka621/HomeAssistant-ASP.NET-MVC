using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAssistant.Infrastructure.Migrations
{
    public partial class added_userActivityLog_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserActivityLogs",
                columns: table => new
                {
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RequestType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RequestUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    QueryString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActionArgumentsJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActivityLogs", x => new { x.UserId, x.DateTime });
                    table.ForeignKey(
                        name: "FK_UserActivityLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEL4twJrXyq3cbS3cT4bZXaQt287hA1ChRyylQ8PsOS2ZTFAcWhKfmw/cpXZlQVCX4A==", "8b3af3bb-3780-40ee-aa09-7ef8107a3827" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserActivityLogs");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEHe/FMyCcQ8JUeO1iFwfp9/GNj9kzkVlZMFUdt4UhfS80/LOwhZnXzNjSUyVumqjLQ==", "f8f10d41-ca45-45e3-8824-dbf835feace5" });
        }
    }
}
