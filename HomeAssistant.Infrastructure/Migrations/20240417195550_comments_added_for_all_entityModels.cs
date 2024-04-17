using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAssistant.Infrastructure.Migrations
{
    public partial class comments_added_for_all_entityModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "StepNumber",
                table: "UsersSteps",
                type: "int",
                nullable: false,
                comment: "Step number on which the user is currently on for the given recipe",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartedOn",
                table: "UsersSteps",
                type: "datetime2",
                nullable: false,
                comment: "When was this recipe step started",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "RecipeId",
                table: "UsersSteps",
                type: "int",
                nullable: false,
                comment: "Recipe identifier",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UsersSteps",
                type: "nvarchar(450)",
                nullable: false,
                comment: "User Identifier",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "StepType",
                table: "Steps",
                type: "int",
                nullable: false,
                comment: "Type of recipe step - timer or task",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Steps",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                comment: "Step name",
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60);

            migrationBuilder.AlterColumn<int>(
                name: "DurationInMin",
                table: "Steps",
                type: "int",
                nullable: true,
                comment: "Duration for the current step if it is a timer",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Steps",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                comment: "Step description",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<int>(
                name: "StepNumber",
                table: "Steps",
                type: "int",
                nullable: false,
                comment: "Step number for the current recipe",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "RecipeId",
                table: "Steps",
                type: "int",
                nullable: false,
                comment: "Recipe step identifier",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "StorePrice",
                table: "ShoppingListsProducts",
                type: "float",
                nullable: true,
                comment: "Optional store price which is useful only for the shopping list",
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "QuantityToBuy",
                table: "ShoppingListsProducts",
                type: "int",
                nullable: false,
                comment: "Quantity to buy which will be transfered to the fridge ones the shopping is done",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "IsBought",
                table: "ShoppingListsProducts",
                type: "bit",
                nullable: false,
                comment: "Is the product currently in the shopping list bought?",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ShoppingListsProducts",
                type: "int",
                nullable: false,
                comment: "Product Identifier",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ShoppingListId",
                table: "ShoppingListsProducts",
                type: "nvarchar(450)",
                nullable: false,
                comment: "Shopping List Identifier",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartedOn",
                table: "ShoppingLists",
                type: "datetime2",
                nullable: true,
                comment: "Shopping list started on",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsStarted",
                table: "ShoppingLists",
                type: "bit",
                nullable: false,
                comment: "Is the shopping list started",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsFinished",
                table: "ShoppingLists",
                type: "bit",
                nullable: false,
                comment: "Is shopping list finished",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ShoppingLists",
                type: "nvarchar(450)",
                nullable: false,
                comment: "User identifier",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "RecipesProductsSteps",
                type: "int",
                nullable: false,
                comment: "Product id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "RecipeId",
                table: "RecipesProductsSteps",
                type: "int",
                nullable: false,
                comment: "Recipe id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "StepNumber",
                table: "RecipesProductsSteps",
                type: "int",
                nullable: false,
                comment: "Recipe step number for which the recipe product is associated with",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "RecipesProducts",
                type: "int",
                nullable: false,
                comment: "Quantity of the selected product required for the recipe",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "RecipesProducts",
                type: "int",
                nullable: false,
                comment: "Id of the product associated with the recipe",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "RecipeId",
                table: "RecipesProducts",
                type: "int",
                nullable: false,
                comment: "Id of the recipe",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Recipes",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                comment: "Recipe Name",
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Recipes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                comment: "Recipe Description",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Recipes",
                type: "int",
                nullable: false,
                comment: "Recipe identifier",
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDismissed",
                table: "NotificationsUsers",
                type: "bit",
                nullable: false,
                comment: "Is notification dismissed by a certain user",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "InvokerURL",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                comment: "The system controller and route from where the create notification was called",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "InvokedBy",
                table: "Notifications",
                type: "nvarchar(450)",
                nullable: true,
                comment: "The user who caused the notification to be generated (could be null)",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MessageContent",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Message text content",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "Messages",
                type: "datetime2",
                nullable: false,
                comment: "Message created on",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "ChatRoomId",
                table: "Messages",
                type: "int",
                nullable: false,
                comment: "Chat room Identifier",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Messages",
                type: "nvarchar(450)",
                nullable: false,
                comment: "User id who wrote the message",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "MessageId",
                table: "Messages",
                type: "int",
                nullable: false,
                comment: "Message Identifier",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "Temperature",
                table: "homeTelemetries",
                type: "float",
                nullable: false,
                comment: "Detected Temperature",
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<double>(
                name: "Radiation",
                table: "homeTelemetries",
                type: "float",
                nullable: false,
                comment: "Calculated radiation in microsilverts",
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<double>(
                name: "Humidity",
                table: "homeTelemetries",
                type: "float",
                nullable: false,
                comment: "Detected humidity",
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateTime",
                table: "homeTelemetries",
                type: "datetime2",
                nullable: false,
                comment: "Date and time of the record",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<double>(
                name: "CPM",
                table: "homeTelemetries",
                type: "float",
                nullable: false,
                comment: "Detected clicks per minute (beta and gama rays only)",
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "homeTelemetries",
                type: "int",
                nullable: false,
                comment: "Telemetry record identifier",
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "User2Id",
                table: "ChatRooms",
                type: "nvarchar(450)",
                nullable: false,
                comment: "User 2 Identifier",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "User1Id",
                table: "ChatRooms",
                type: "nvarchar(450)",
                nullable: false,
                comment: "User 1 Identifier",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ChatRooms",
                type: "int",
                nullable: false,
                comment: "Chat romm identifier",
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Category name",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Categories",
                type: "int",
                nullable: false,
                comment: "Category Identifier",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Category identifier")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "AspNetUsers",
                type: "float",
                nullable: false,
                defaultValue: 23.319941,
                comment: "User location (Longitude) - default is in Sofia/Bulgaria",
                oldClrType: typeof(double),
                oldType: "float",
                oldDefaultValue: 23.319941);

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "AspNetUsers",
                type: "float",
                nullable: false,
                defaultValue: 42.698334000000003,
                comment: "User location (Latitude) - default is in Sofia/Bulgaria",
                oldClrType: typeof(double),
                oldType: "float",
                oldDefaultValue: 42.698334000000003);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                comment: "Last name",
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                comment: "Is the user deleted",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                comment: "First name",
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletedOn",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true,
                comment: "User delition date if the user is deleted",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                comment: "User creation date",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEOq/sU+MfuR3RqHsuVJ1U2M9iZZsJgk+dQoQCbsIFB0CQLZ0XHj2eSO2BkmuSYILtw==", "854963e2-a0ce-460f-93b3-a6af81e23ef5" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "StepNumber",
                table: "UsersSteps",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Step number on which the user is currently on for the given recipe");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartedOn",
                table: "UsersSteps",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "When was this recipe step started");

            migrationBuilder.AlterColumn<int>(
                name: "RecipeId",
                table: "UsersSteps",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Recipe identifier");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UsersSteps",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldComment: "User Identifier");

            migrationBuilder.AlterColumn<int>(
                name: "StepType",
                table: "Steps",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Type of recipe step - timer or task");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Steps",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60,
                oldComment: "Step name");

            migrationBuilder.AlterColumn<int>(
                name: "DurationInMin",
                table: "Steps",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Duration for the current step if it is a timer");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Steps",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldComment: "Step description");

            migrationBuilder.AlterColumn<int>(
                name: "StepNumber",
                table: "Steps",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Step number for the current recipe");

            migrationBuilder.AlterColumn<int>(
                name: "RecipeId",
                table: "Steps",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Recipe step identifier");

            migrationBuilder.AlterColumn<double>(
                name: "StorePrice",
                table: "ShoppingListsProducts",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true,
                oldComment: "Optional store price which is useful only for the shopping list");

            migrationBuilder.AlterColumn<int>(
                name: "QuantityToBuy",
                table: "ShoppingListsProducts",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Quantity to buy which will be transfered to the fridge ones the shopping is done");

            migrationBuilder.AlterColumn<bool>(
                name: "IsBought",
                table: "ShoppingListsProducts",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Is the product currently in the shopping list bought?");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ShoppingListsProducts",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Product Identifier");

            migrationBuilder.AlterColumn<string>(
                name: "ShoppingListId",
                table: "ShoppingListsProducts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldComment: "Shopping List Identifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartedOn",
                table: "ShoppingLists",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Shopping list started on");

            migrationBuilder.AlterColumn<bool>(
                name: "IsStarted",
                table: "ShoppingLists",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Is the shopping list started");

            migrationBuilder.AlterColumn<bool>(
                name: "IsFinished",
                table: "ShoppingLists",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Is shopping list finished");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ShoppingLists",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldComment: "User identifier");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "RecipesProductsSteps",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Product id");

            migrationBuilder.AlterColumn<int>(
                name: "RecipeId",
                table: "RecipesProductsSteps",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Recipe id");

            migrationBuilder.AlterColumn<int>(
                name: "StepNumber",
                table: "RecipesProductsSteps",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Recipe step number for which the recipe product is associated with");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "RecipesProducts",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Quantity of the selected product required for the recipe");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "RecipesProducts",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Id of the product associated with the recipe");

            migrationBuilder.AlterColumn<int>(
                name: "RecipeId",
                table: "RecipesProducts",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Id of the recipe");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Recipes",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60,
                oldComment: "Recipe Name");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Recipes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldComment: "Recipe Description");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Recipes",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Recipe identifier")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDismissed",
                table: "NotificationsUsers",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Is notification dismissed by a certain user");

            migrationBuilder.AlterColumn<string>(
                name: "InvokerURL",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "The system controller and route from where the create notification was called");

            migrationBuilder.AlterColumn<string>(
                name: "InvokedBy",
                table: "Notifications",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true,
                oldComment: "The user who caused the notification to be generated (could be null)");

            migrationBuilder.AlterColumn<string>(
                name: "MessageContent",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Message text content");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "Messages",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Message created on");

            migrationBuilder.AlterColumn<int>(
                name: "ChatRoomId",
                table: "Messages",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Chat room Identifier");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Messages",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldComment: "User id who wrote the message");

            migrationBuilder.AlterColumn<int>(
                name: "MessageId",
                table: "Messages",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Message Identifier");

            migrationBuilder.AlterColumn<double>(
                name: "Temperature",
                table: "homeTelemetries",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float",
                oldComment: "Detected Temperature");

            migrationBuilder.AlterColumn<double>(
                name: "Radiation",
                table: "homeTelemetries",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float",
                oldComment: "Calculated radiation in microsilverts");

            migrationBuilder.AlterColumn<double>(
                name: "Humidity",
                table: "homeTelemetries",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float",
                oldComment: "Detected humidity");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateTime",
                table: "homeTelemetries",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date and time of the record");

            migrationBuilder.AlterColumn<double>(
                name: "CPM",
                table: "homeTelemetries",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float",
                oldComment: "Detected clicks per minute (beta and gama rays only)");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "homeTelemetries",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Telemetry record identifier")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "User2Id",
                table: "ChatRooms",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldComment: "User 2 Identifier");

            migrationBuilder.AlterColumn<string>(
                name: "User1Id",
                table: "ChatRooms",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldComment: "User 1 Identifier");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ChatRooms",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Chat romm identifier")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Category name");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Categories",
                type: "int",
                nullable: false,
                comment: "Category identifier",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Category Identifier")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "AspNetUsers",
                type: "float",
                nullable: false,
                defaultValue: 23.319941,
                oldClrType: typeof(double),
                oldType: "float",
                oldDefaultValue: 23.319941,
                oldComment: "User location (Longitude) - default is in Sofia/Bulgaria");

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "AspNetUsers",
                type: "float",
                nullable: false,
                defaultValue: 42.698334000000003,
                oldClrType: typeof(double),
                oldType: "float",
                oldDefaultValue: 42.698334000000003,
                oldComment: "User location (Latitude) - default is in Sofia/Bulgaria");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60,
                oldComment: "Last name");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Is the user deleted");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60,
                oldComment: "First name");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletedOn",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "User delition date if the user is deleted");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "User creation date");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2246145-9dd8-4902-ae41-68096b5ca738",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEFNeSp8kPANRsiihmwCjz57l8bphILoIi68h9NuLmyHVhFiLeM0kp/UmYxtsp6Mbig==", "9c041d08-cd4d-451a-ab00-d704b83e693c" });
        }
    }
}
