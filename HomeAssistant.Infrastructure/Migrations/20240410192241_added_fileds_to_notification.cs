using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAssistant.Infrastructure.Migrations
{
    public partial class added_fileds_to_notification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InvokedBy",
                table: "Notifications",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvokerURL",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEGxV7PLyUUiBa88xv0aVoAWY92IgO1I+TdunkxowcH0rgOemACNivFfEIPOvBXRDTQ==", "753fec1f-e7c3-4149-81bb-4e7dcb1341a2" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_InvokedBy",
                table: "Notifications",
                column: "InvokedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_InvokedBy",
                table: "Notifications",
                column: "InvokedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_InvokedBy",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_InvokedBy",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "InvokedBy",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "InvokerURL",
                table: "Notifications");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAELlQZCplK1vqeDGfrf10NZUqaszGoWl1RPqWLJVFiqerLc88oNp85zgKpOiNoZE5tA==", "9e644a4e-9e00-4ac1-b509-acb1a7a87aab" });
        }
    }
}
