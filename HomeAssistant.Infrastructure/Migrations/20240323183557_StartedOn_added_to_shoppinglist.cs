using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAssistant.Infrastructure.Migrations
{
    public partial class StartedOn_added_to_shoppinglist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "StartedOn",
                table: "ShoppingLists",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAELqyLmlp4WDhv8zmP+8v++ZWgYvDP8WqnNZaV0pwN8IE6x4/5oOPwv6lFIHBpAb95w==", "b4185ecc-fb03-4c60-b0da-6adbebf11763" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartedOn",
                table: "ShoppingLists");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAECt515VQygFos9AtPpgh2jwkfX+tSYqEopTnZREhVYB0PMOXePjtj+S9GyLfHe1v8w==", "28f37f11-37b6-4cbf-8502-1d0fbcf1c64d" });
        }
    }
}
