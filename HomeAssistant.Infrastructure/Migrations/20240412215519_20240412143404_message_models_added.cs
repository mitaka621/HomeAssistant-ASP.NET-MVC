using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAssistant.Infrastructure.Migrations
{
    public partial class _20240412143404_message_models_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Notification title",
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60,
                oldComment: "Notification title");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                comment: "Notification description",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "Notification description (optional)");

            migrationBuilder.CreateTable(
                name: "ChatRooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User1Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    User2Id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatRooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatRooms_AspNetUsers_User1Id",
                        column: x => x.User1Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ChatRooms_AspNetUsers_User2Id",
                        column: x => x.User2Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "int", nullable: false),
                    ChatRoomId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MessageContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => new { x.MessageId, x.UserId, x.ChatRoomId });
                    table.ForeignKey(
                        name: "FK_Messages_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Messages_ChatRooms_ChatRoomId",
                        column: x => x.ChatRoomId,
                        principalTable: "ChatRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEPAQKGu1mZCXtWqIV4eGqRh04ABLJ1sOacG9RaEgRF28M+NWdCICq05MC1bxj0FZjw==", "b12bd6cc-e77b-44ef-960a-ef8641bd5ef3" });

            migrationBuilder.CreateIndex(
                name: "IX_ChatRooms_User1Id",
                table: "ChatRooms",
                column: "User1Id");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRooms_User2Id",
                table: "ChatRooms",
                column: "User2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatRoomId",
                table: "Messages",
                column: "ChatRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UserId",
                table: "Messages",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "ChatRooms");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Notifications",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                comment: "Notification title",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Notification title");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Notifications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "Notification description (optional)",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Notification description");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEAOrRsRSsrAtdtKzYZxTxlyvI599MTRdP4j3oS4LW95UbKy2bxObyVPg+gAch8OiHA==", "9d4b97e2-ffd7-42a3-8ee9-ba467c425491" });
        }
    }
}
