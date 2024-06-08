using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAssistant.Infrastructure.Migrations
{
    public partial class added_fields_required_for_pushNotifications_HomeAssistantUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ClientIpAddress",
                table: "AspNetUsers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "P256dh",
                table: "AspNetUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PushNotificationAuth",
                table: "AspNetUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PushNotificationEndpoint",
                table: "AspNetUsers",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEMwj8J0RzzmrBFAHUyf6EpDBbKSjwH44Tfg5mroFdbBgq0evILNRgYQKQfoiix4cJw==", "74083655-c67a-47ad-b251-91492f3c8d47" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "P256dh",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PushNotificationAuth",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PushNotificationEndpoint",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "ClientIpAddress",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEL4twJrXyq3cbS3cT4bZXaQt287hA1ChRyylQ8PsOS2ZTFAcWhKfmw/cpXZlQVCX4A==", "8b3af3bb-3780-40ee-aa09-7ef8107a3827" });
        }
    }
}
