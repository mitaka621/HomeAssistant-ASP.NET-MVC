using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAssistant.Infrastructure.Migrations
{
    public partial class chnaged_primaryKey_HomeTelemetry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_homeTelemetries",
                table: "homeTelemetries");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "homeTelemetries");

            migrationBuilder.AddPrimaryKey(
                name: "PK_homeTelemetries",
                table: "homeTelemetries",
                column: "DateTime");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAELubfLYxBZxu4n7lsHMfbQ98G/LTCQ1l53X2/AallPbjiZYljQ3mvJhCujL0PVGDWA==", "d86f6a03-caec-4e5f-b2a6-668831c53095" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_homeTelemetries",
                table: "homeTelemetries");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "homeTelemetries",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Telemetry record identifier")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_homeTelemetries",
                table: "homeTelemetries",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAELoDcJ30OJg3Ndnl3ka3y7SkXHwR3LqajEN7hoT8RH9ENkLxBcQGRTH8+tU+Z/X6jQ==", "88df07f0-44d8-4d0d-b6a5-461ab1b2323d" });
        }
    }
}
