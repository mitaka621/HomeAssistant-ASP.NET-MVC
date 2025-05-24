using Microsoft.EntityFrameworkCore;

namespace HomeAssistant.Extensions
{
    public static class WebApplicationExtensions
    {
        public static void MigrateDatabase<TContext>(this IHost app) where TContext : DbContext
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();
                dbContext.Database.Migrate();
            }
        }

    }
}
