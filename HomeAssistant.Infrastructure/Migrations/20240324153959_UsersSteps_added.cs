using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAssistant.Infrastructure.Migrations
{
    public partial class UsersSteps_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsersSteps",
                columns: table => new
                {
                    RecipeId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StepNumber = table.Column<int>(type: "int", nullable: false),
                    StartedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersSteps", x => new { x.UserId, x.RecipeId });
                    table.ForeignKey(
                        name: "FK_UsersSteps_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersSteps_Steps_RecipeId_StepNumber",
                        columns: x => new { x.RecipeId, x.StepNumber },
                        principalTable: "Steps",
                        principalColumns: new[] { "RecipeId", "StepNumber" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEFLiQWaxihsh3fKP4l32vODaQXCsqb2gdADZ4uozG2678T6l1WcrqkI0ylHIC2pbYQ==", "58a0869c-1d43-4be5-9eda-191f48d4f9ea" });

            migrationBuilder.CreateIndex(
                name: "IX_UsersSteps_RecipeId_StepNumber",
                table: "UsersSteps",
                columns: new[] { "RecipeId", "StepNumber" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsersSteps");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEPV9XoD4X9kNuRNpQwXIrTPs8xsXZ3vqluplhQn7r2UcbeV1T0bt43Vl7n3k7azWag==", "9d8ee545-62a5-4c4e-be8a-df7766f75ad8" });
        }
    }
}
