using HomeAssistant.Infrastructure.Data.Models;
using HomeAssistant.Infrastructure.Data.SeedDb;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace HomeAssistant.Infrastructure.Data
{
    public class HomeAssistantDbContext : IdentityDbContext<HomeAssistantUser>
    {
        public HomeAssistantDbContext(DbContextOptions<HomeAssistantDbContext> options)
            : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationUser> NotificationsUsers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ShoppingList> ShoppingLists { get; set; }
        public DbSet<ShoppingListProduct> ShoppingListsProducts { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeProduct> RecipesProducts { get; set; }
        public DbSet<Step> Steps { get; set; }
		public DbSet<UserStep> UsersSteps { get; set; }
        public DbSet<RecipeProductStep> RecipesProductsSteps { get; set; }
        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<HomeTelemetry> homeTelemetries { get; set; }
		public DbSet<BlacklistedIp> BlacklistedIPs { get; set; }
		public DbSet<UserActivityLog> UserActivityLogs { get; set; }
        public DbSet<UserSubscribtionData> UserSubscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ShoppingListProduct>()
                .HasKey(x => new { x.ShoppingListId, x.ProductId });

            builder.Entity<NotificationUser>()
                .HasKey(x => new { x.UserId, x.NotificationId });

            builder.Entity<RecipeProduct>()
                .HasKey(x => new { x.RecipeId, x.ProductId });

            builder.Entity<Step>()
                .HasKey(x => new { x.RecipeId, x.StepNumber });

            builder.Entity<UserStep>()
                .HasKey(x => new { x.UserId,x.RecipeId});

            builder.Entity<RecipeProductStep>()
                .HasKey(x => new { x.StepNumber,x.RecipeId,x.ProductId});

            builder.Entity<Message>()
                .HasKey(x => new { x.MessageId, x.UserId, x.ChatRoomId });

			builder.Entity<UserActivityLog>()
			   .HasKey(x => new { x.UserId, x.DateTime });

            builder.Entity<UserSubscribtionData>()
                .HasKey(x => new { x.UserId, x.DeviceType });

			builder.Entity<HomeAssistantUser>()
                .Property(b => b.Latitude)
                .HasDefaultValue(42.698334);

            builder.Entity<HomeAssistantUser>()
               .Property(b => b.Longitude)
               .HasDefaultValue(23.319941);
         
            builder.Entity<HomeAssistantUser>()
           .HasOne(f => f.ShoppingList)
           .WithOne(s => s.User)
           .HasForeignKey<ShoppingList>(s => s.UserId);

            builder.ApplyConfiguration(new AdminConfiguration());

            builder.ApplyConfiguration(new RoleConfiguration());

            builder.ApplyConfiguration(new UserRolesConfiguration());

            builder.ApplyConfiguration(new CategoryConfiguration());

            builder.ApplyConfiguration(new ProductsConfiguration());

            base.OnModelCreating(builder);
        }
    }
}
