using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAssistant.Infrastructure.Migrations
{
    public partial class added_ShoppingListProduct_price_and_quantityToBuy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuantityToBuy",
                table: "ShoppingListsProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "StorePrice",
                table: "ShoppingListsProducts",
                type: "float",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAECt515VQygFos9AtPpgh2jwkfX+tSYqEopTnZREhVYB0PMOXePjtj+S9GyLfHe1v8w==", "28f37f11-37b6-4cbf-8502-1d0fbcf1c64d" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuantityToBuy",
                table: "ShoppingListsProducts");

            migrationBuilder.DropColumn(
                name: "StorePrice",
                table: "ShoppingListsProducts");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEAF4Kai2uL14VOWrUZxyqHo5HEW76gDK2R0g3jB6kFNAOQuSORP4PsyXqNEw4u8ngw==", "280936f2-438d-4417-a2d5-836006b10247" });
        }
    }
}
