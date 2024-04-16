using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAssistant.Infrastructure.Migrations
{
    public partial class HomeTelemetry_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "homeTelemetries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Humidity = table.Column<int>(type: "int", nullable: false),
                    Tempreture = table.Column<int>(type: "int", nullable: false),
                    CPM = table.Column<int>(type: "int", nullable: false),
                    Radiation = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_homeTelemetries", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEBs6weMiVcuBAsQf+F3LCoaD6JLzP+by4LZ9WeDf3+/c5Jg+z0DYgL7E4mfcRRUZOA==", "ca61ac28-0319-4e69-af13-d96e03797cff" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "homeTelemetries");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEPAQKGu1mZCXtWqIV4eGqRh04ABLJ1sOacG9RaEgRF28M+NWdCICq05MC1bxj0FZjw==", "b12bd6cc-e77b-44ef-960a-ef8641bd5ef3" });
        }
    }
}
