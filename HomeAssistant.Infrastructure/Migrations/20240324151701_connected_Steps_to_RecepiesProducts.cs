using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAssistant.Infrastructure.Migrations
{
    public partial class connected_Steps_to_RecepiesProducts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StepNumber",
                table: "RecipesProducts",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEPV9XoD4X9kNuRNpQwXIrTPs8xsXZ3vqluplhQn7r2UcbeV1T0bt43Vl7n3k7azWag==", "9d8ee545-62a5-4c4e-be8a-df7766f75ad8" });

            migrationBuilder.CreateIndex(
                name: "IX_RecipesProducts_RecipeId_StepNumber",
                table: "RecipesProducts",
                columns: new[] { "RecipeId", "StepNumber" });

            migrationBuilder.AddForeignKey(
                name: "FK_RecipesProducts_Steps_RecipeId_StepNumber",
                table: "RecipesProducts",
                columns: new[] { "RecipeId", "StepNumber" },
                principalTable: "Steps",
                principalColumns: new[] { "RecipeId", "StepNumber" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecipesProducts_Steps_RecipeId_StepNumber",
                table: "RecipesProducts");

            migrationBuilder.DropIndex(
                name: "IX_RecipesProducts_RecipeId_StepNumber",
                table: "RecipesProducts");

            migrationBuilder.DropColumn(
                name: "StepNumber",
                table: "RecipesProducts");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAECesmfOwNZwis0Dhed51IDaRQ1JtMzHYQmkF7A+14XZZ5VHuhi+kWYKar3RjlDmspQ==", "91532d95-9226-4f0a-941c-40018dcf0738" });
        }
    }
}
