using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAssistant.Infrastructure.Migrations
{
    public partial class added_UserSubscription_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "UserSubscriptions",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DeviceType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PushNotificationEndpoint = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    PushNotificationAuth = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    P256dh = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSubscriptions", x => new { x.UserId, x.DeviceType });
                    table.ForeignKey(
                        name: "FK_UserSubscriptions_AspNetUsers_UserId",
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
                values: new object[] { "AQAAAAEAACcQAAAAEIUJk0F66TYdPC3ZIdU1tlBsg8QIwKuQgmaRRin+bnM4vnCHe4BbnUnSRojYrwQjtA==", "823513d7-82b8-4fd4-8bb2-3fc52989d85b" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSubscriptions");

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
    }
}
