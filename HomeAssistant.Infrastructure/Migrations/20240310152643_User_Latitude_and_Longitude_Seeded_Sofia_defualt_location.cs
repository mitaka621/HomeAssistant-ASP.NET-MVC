using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAssistant.Infrastructure.Migrations
{
    public partial class User_Latitude_and_Longitude_Seeded_Sofia_defualt_location : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "AspNetUsers",
                type: "float",
                nullable: false,
                defaultValue: 42.698334000000003);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "AspNetUsers",
                type: "float",
                nullable: false,
                defaultValue: 23.319941);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEB8vX8zxqtgljTu6MII0LQEqf8vGrsZ8BQonxzn/+MEMFsNKhbU6DB+eKMiAvshXxw==", "bf796b48-44cc-4f8f-b1eb-3c9ae06b7490" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEDv+rDRWIbxzY+zFSMOQFad9u/nYz5Mxra/RmVRlyg0rtVVj7VCR7D80JKmE70F9xA==", "c0d7d2d2-527d-426c-ba11-cdbdf90a2eae" });
        }
    }
}
