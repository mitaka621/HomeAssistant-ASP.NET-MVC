using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HomeAssistant.Infrastructure.Data
{
    public class HomeAssistantDbContext : IdentityDbContext
    {
        public HomeAssistantDbContext(DbContextOptions<HomeAssistantDbContext> options)
            : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationUser> NotificationsUsers { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<NotificationUser>()
                .HasKey(x => new { x.UserId, x.NotificationId });

            base.OnModelCreating(builder);
        }
    }
}
