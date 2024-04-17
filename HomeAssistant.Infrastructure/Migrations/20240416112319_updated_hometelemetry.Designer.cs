﻿// <auto-generated />
using System;
using HomeAssistant.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HomeAssistant.Infrastructure.Migrations
{
    [DbContext(typeof(HomeAssistantDbContext))]
    [Migration("20240416112319_updated_hometelemetry")]
    partial class updated_hometelemetry
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.27")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasComment("Category identifier");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Categories");

                    b.HasComment("Product category");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Dairy"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Fruits"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Meat and Poultry"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Beverages"
                        },
                        new
                        {
                            Id = 5,
                            Name = "Vegetables"
                        },
                        new
                        {
                            Id = 6,
                            Name = "Baking Supplies"
                        },
                        new
                        {
                            Id = 7,
                            Name = "Condiments and Sauces"
                        });
                });

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.ChatRoom", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("User1Id")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("User2Id")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("User1Id");

                    b.HasIndex("User2Id");

                    b.ToTable("ChatRooms");
                });

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.HomeAssistantUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("nvarchar(60)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("nvarchar(60)");

                    b.Property<double>("Latitude")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("float")
                        .HasDefaultValue(42.698334000000003);

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<double>("Longitude")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("float")
                        .HasDefaultValue(23.319941);

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);

                    b.HasData(
                        new
                        {
                            Id = "e2246145-9dd8-4902-ae41-68096b5ca738",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "e2246145-9dd8-4902-ae41-68096b5ca738",
                            CreatedOn = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "admin",
                            EmailConfirmed = false,
                            FirstName = "",
                            IsDeleted = false,
                            LastName = "",
                            Latitude = 0.0,
                            LockoutEnabled = false,
                            Longitude = 0.0,
                            NormalizedEmail = "admin",
                            NormalizedUserName = "admin",
                            PasswordHash = "AQAAAAEAACcQAAAAEFNeSp8kPANRsiihmwCjz57l8bphILoIi68h9NuLmyHVhFiLeM0kp/UmYxtsp6Mbig==",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "9c041d08-cd4d-451a-ab00-d704b83e693c",
                            TwoFactorEnabled = false,
                            UserName = "admin"
                        });
                });

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.HomeTelemetry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<double>("CPM")
                        .HasColumnType("float");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<double>("Humidity")
                        .HasColumnType("float");

                    b.Property<double>("Radiation")
                        .HasColumnType("float");

                    b.Property<double>("Temperature")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("homeTelemetries");
                });

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.Message", b =>
                {
                    b.Property<int>("MessageId")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("ChatRoomId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("MessageContent")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("MessageId", "UserId", "ChatRoomId");

                    b.HasIndex("ChatRoomId");

                    b.HasIndex("UserId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasComment("Notification Identifier");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2")
                        .HasComment("Date and time when the notification was created");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasComment("Notification description");

                    b.Property<string>("InvokedBy")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("InvokerURL")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasComment("Notification title");

                    b.HasKey("Id");

                    b.HasIndex("InvokedBy");

                    b.ToTable("Notifications");

                    b.HasComment("Notification");
                });

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.NotificationUser", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)")
                        .HasComment("User Identifier");

                    b.Property<int>("NotificationId")
                        .HasColumnType("int")
                        .HasComment("Notification Identifier");

                    b.Property<bool>("IsDismissed")
                        .HasColumnType("bit");

                    b.HasKey("UserId", "NotificationId");

                    b.HasIndex("NotificationId");

                    b.ToTable("NotificationsUsers");

                    b.HasComment("Which user has seen what notification");
                });

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasComment("Product identifier");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("AddedOn")
                        .HasColumnType("datetime2")
                        .HasComment("Date and time for when the product was added");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int")
                        .HasComment("Product category");

                    b.Property<int>("Count")
                        .HasColumnType("int")
                        .HasComment("Product Count");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("nvarchar(60)")
                        .HasComment("Product name");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)")
                        .HasComment("UserId who added the product (optional)");

                    b.Property<int?>("Weight")
                        .HasColumnType("int")
                        .HasComment("Product weight in grams (optional)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("UserId");

                    b.ToTable("Products");

                    b.HasComment("Product");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            AddedOn = new DateTime(2024, 3, 14, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            CategoryId = 3,
                            Count = 3,
                            Name = "Steak"
                        },
                        new
                        {
                            Id = 2,
                            AddedOn = new DateTime(2024, 3, 14, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            CategoryId = 2,
                            Count = 10,
                            Name = "Apple"
                        });
                });

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.Recipe", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("nvarchar(60)");

                    b.HasKey("Id");

                    b.ToTable("Recipes");
                });

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.RecipeProduct", b =>
                {
                    b.Property<int>("RecipeId")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("RecipeId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("RecipesProducts");
                });

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.RecipeProductStep", b =>
                {
                    b.Property<int>("StepNumber")
                        .HasColumnType("int");

                    b.Property<int>("RecipeId")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.HasKey("StepNumber", "RecipeId", "ProductId");

                    b.HasIndex("RecipeId", "ProductId");

                    b.HasIndex("RecipeId", "StepNumber");

                    b.ToTable("RecipesProductsSteps");
                });

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.ShoppingList", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsFinished")
                        .HasColumnType("bit");

                    b.Property<bool>("IsStarted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("StartedOn")
                        .HasColumnType("datetime2");

                    b.HasKey("UserId");

                    b.ToTable("ShoppingLists");
                });

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.ShoppingListProduct", b =>
                {
                    b.Property<string>("ShoppingListId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<bool>("IsBought")
                        .HasColumnType("bit");

                    b.Property<int>("QuantityToBuy")
                        .HasColumnType("int");

                    b.Property<double?>("StorePrice")
                        .HasColumnType("float");

                    b.HasKey("ShoppingListId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("ShoppingListsProducts");
                });

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.Step", b =>
                {
                    b.Property<int>("RecipeId")
                        .HasColumnType("int");

                    b.Property<int>("StepNumber")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int?>("DurationInMin")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("nvarchar(60)");

                    b.Property<int>("StepType")
                        .HasColumnType("int");

                    b.HasKey("RecipeId", "StepNumber");

                    b.ToTable("Steps");
                });

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.UserStep", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("RecipeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartedOn")
                        .HasColumnType("datetime2");

                    b.Property<int>("StepNumber")
                        .HasColumnType("int");

                    b.HasKey("UserId", "RecipeId");

                    b.HasIndex("RecipeId", "StepNumber");

                    b.ToTable("UsersSteps");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = "7e2d4805-c978-4600-9663-a9cafa2a54be",
                            ConcurrencyStamp = "7e2d4805-c978-4600-9663-a9cafa2a54be",
                            Name = "StandardUser",
                            NormalizedName = "StandardUser"
                        },
                        new
                        {
                            Id = "f23e50cd-3de0-4420-ae9b-6ce529f3128f",
                            ConcurrencyStamp = "f23e50cd-3de0-4420-ae9b-6ce529f3128f",
                            Name = "Admin",
                            NormalizedName = "Admin"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);

                    b.HasData(
                        new
                        {
                            UserId = "e2246145-9dd8-4902-ae41-68096b5ca738",
                            RoleId = "f23e50cd-3de0-4420-ae9b-6ce529f3128f"
                        },
                        new
                        {
                            UserId = "e2246145-9dd8-4902-ae41-68096b5ca738",
                            RoleId = "7e2d4805-c978-4600-9663-a9cafa2a54be"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.ChatRoom", b =>
                {
                    b.HasOne("HomeAssistant.Infrastructure.Data.Models.HomeAssistantUser", "User1")
                        .WithMany()
                        .HasForeignKey("User1Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HomeAssistant.Infrastructure.Data.Models.HomeAssistantUser", "User2")
                        .WithMany()
                        .HasForeignKey("User2Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User1");

                    b.Navigation("User2");
                });

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.Message", b =>
                {
                    b.HasOne("HomeAssistant.Infrastructure.Data.Models.ChatRoom", "ChatRoom")
                        .WithMany("Messages")
                        .HasForeignKey("ChatRoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HomeAssistant.Infrastructure.Data.Models.HomeAssistantUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ChatRoom");

                    b.Navigation("User");
                });

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.Notification", b =>
                {
                    b.HasOne("HomeAssistant.Infrastructure.Data.Models.HomeAssistantUser", "User")
                        .WithMany("InvokedNotifications")
                        .HasForeignKey("InvokedBy");

                    b.Navigation("User");
                });

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.NotificationUser", b =>
                {
                    b.HasOne("HomeAssistant.Infrastructure.Data.Models.Notification", "Notification")
                        .WithMany("NotificationsUsers")
                        .HasForeignKey("NotificationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HomeAssistant.Infrastructure.Data.Models.HomeAssistantUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Notification");

                    b.Navigation("User");
                });

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.Product", b =>
                {
                    b.HasOne("HomeAssistant.Infrastructure.Data.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HomeAssistant.Infrastructure.Data.Models.HomeAssistantUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("Category");

                    b.Navigation("User");
                });

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.RecipeProduct", b =>
                {
                    b.HasOne("HomeAssistant.Infrastructure.Data.Models.Product", "Product")
                        .WithMany("ProductRecipes")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HomeAssistant.Infrastructure.Data.Models.Recipe", "Recipe")
                        .WithMany("RecipeProducts")
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("Recipe");
                });

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.RecipeProductStep", b =>
                {
                    b.HasOne("HomeAssistant.Infrastructure.Data.Models.RecipeProduct", "RecipeProduct")
                        .WithMany("Steps")
                        .HasForeignKey("RecipeId", "ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HomeAssistant.Infrastructure.Data.Models.Step", "Step")
                        .WithMany("RecipeProductStep")
                        .HasForeignKey("RecipeId", "StepNumber")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RecipeProduct");

                    b.Navigation("Step");
                });

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.ShoppingList", b =>
                {
                    b.HasOne("HomeAssistant.Infrastructure.Data.Models.HomeAssistantUser", "User")
                        .WithOne("ShoppingList")
                        .HasForeignKey("HomeAssistant.Infrastructure.Data.Models.ShoppingList", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.ShoppingListProduct", b =>
                {
                    b.HasOne("HomeAssistant.Infrastructure.Data.Models.Product", "Product")
                        .WithMany("ProductShoppingLists")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HomeAssistant.Infrastructure.Data.Models.ShoppingList", "ShoppingList")
                        .WithMany("ShoppingListProducts")
                        .HasForeignKey("ShoppingListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("ShoppingList");
                });

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.Step", b =>
                {
                    b.HasOne("HomeAssistant.Infrastructure.Data.Models.Recipe", "Recipe")
                        .WithMany("Steps")
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Recipe");
                });

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.UserStep", b =>
                {
                    b.HasOne("HomeAssistant.Infrastructure.Data.Models.HomeAssistantUser", "User")
                        .WithMany("UserRecipeSteps")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HomeAssistant.Infrastructure.Data.Models.Step", "Step")
                        .WithMany()
                        .HasForeignKey("RecipeId", "StepNumber")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Step");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("HomeAssistant.Infrastructure.Data.Models.HomeAssistantUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("HomeAssistant.Infrastructure.Data.Models.HomeAssistantUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HomeAssistant.Infrastructure.Data.Models.HomeAssistantUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("HomeAssistant.Infrastructure.Data.Models.HomeAssistantUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.ChatRoom", b =>
                {
                    b.Navigation("Messages");
                });

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.HomeAssistantUser", b =>
                {
                    b.Navigation("InvokedNotifications");

                    b.Navigation("ShoppingList")
                        .IsRequired();

                    b.Navigation("UserRecipeSteps");
                });

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.Notification", b =>
                {
                    b.Navigation("NotificationsUsers");
                });

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.Product", b =>
                {
                    b.Navigation("ProductRecipes");

                    b.Navigation("ProductShoppingLists");
                });

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.Recipe", b =>
                {
                    b.Navigation("RecipeProducts");

                    b.Navigation("Steps");
                });

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.RecipeProduct", b =>
                {
                    b.Navigation("Steps");
                });

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.ShoppingList", b =>
                {
                    b.Navigation("ShoppingListProducts");
                });

            modelBuilder.Entity("HomeAssistant.Infrastructure.Data.Models.Step", b =>
                {
                    b.Navigation("RecipeProductStep");
                });
#pragma warning restore 612, 618
        }
    }
}