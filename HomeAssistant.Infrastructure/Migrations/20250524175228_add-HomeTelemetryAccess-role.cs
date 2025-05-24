using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAssistant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addHomeTelemetryAccessrole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "0791d7e9-e680-41a2-ba2b-74009bd8e989", "0791d7e9-e680-41a2-ba2b-74009bd8e989", "HomeTelemetryAccess", "HomeTelemetryAccess" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAIAAYagAAAAEKXamOPbkV1peqOgMZatyDO74ud+GHnUrLV00woEGhBW6iV95h3+BxACGs5B98Qx4g==", "1449f701-b647-4111-b9f3-52be6f43b0b6" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0791d7e9-e680-41a2-ba2b-74009bd8e989");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAENaIclICiv+yP3AtRou44Op2ctSq3vI12MbnEE1+shFpr1qf5bqjasLLK+2QEERh/w==", "bf8155ec-0d49-4a65-8df3-2c24532e1368" });
        }
    }
}
