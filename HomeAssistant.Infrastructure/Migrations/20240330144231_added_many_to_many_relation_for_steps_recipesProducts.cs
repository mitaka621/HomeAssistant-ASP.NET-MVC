using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAssistant.Infrastructure.Migrations
{
    public partial class added_many_to_many_relation_for_steps_recipesProducts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "RecipesProductsSteps",
                columns: table => new
                {
                    RecipeId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    StepNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipesProductsSteps", x => new { x.StepNumber, x.RecipeId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_RecipesProductsSteps_RecipesProducts_RecipeId_ProductId",
                        columns: x => new { x.RecipeId, x.ProductId },
                        principalTable: "RecipesProducts",
                        principalColumns: new[] { "RecipeId", "ProductId" },
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_RecipesProductsSteps_Steps_RecipeId_StepNumber",
                        columns: x => new { x.RecipeId, x.StepNumber },
                        principalTable: "Steps",
                        principalColumns: new[] { "RecipeId", "StepNumber" },
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAELlQZCplK1vqeDGfrf10NZUqaszGoWl1RPqWLJVFiqerLc88oNp85zgKpOiNoZE5tA==", "9e644a4e-9e00-4ac1-b509-acb1a7a87aab" });

            migrationBuilder.CreateIndex(
                name: "IX_RecipesProductsSteps_RecipeId_ProductId",
                table: "RecipesProductsSteps",
                columns: new[] { "RecipeId", "ProductId" });

            migrationBuilder.CreateIndex(
                name: "IX_RecipesProductsSteps_RecipeId_StepNumber",
                table: "RecipesProductsSteps",
                columns: new[] { "RecipeId", "StepNumber" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipesProductsSteps");

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
                values: new object[] { "AQAAAAEAACcQAAAAEFLiQWaxihsh3fKP4l32vODaQXCsqb2gdADZ4uozG2678T6l1WcrqkI0ylHIC2pbYQ==", "58a0869c-1d43-4be5-9eda-191f48d4f9ea" });

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
    }
}
