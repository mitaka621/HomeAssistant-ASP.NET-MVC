using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAssistant.Infrastructure.Migrations
{
    public partial class added_field_IsDismissed_NotificationUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDismissed",
                table: "NotificationsUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEAOrRsRSsrAtdtKzYZxTxlyvI599MTRdP4j3oS4LW95UbKy2bxObyVPg+gAch8OiHA==", "9d4b97e2-ffd7-42a3-8ee9-ba467c425491" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDismissed",
                table: "NotificationsUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEGxV7PLyUUiBa88xv0aVoAWY92IgO1I+TdunkxowcH0rgOemACNivFfEIPOvBXRDTQ==", "753fec1f-e7c3-4149-81bb-4e7dcb1341a2" });
        }
    }
}
