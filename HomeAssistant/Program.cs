using HomeAssistant.BackgroundServiceJobs;
using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Services;
using HomeAssistant.Extensions;
using HomeAssistant.Filters;
using HomeAssistant.Hubs;
using HomeAssistant.Infrastructure.Data;
using HomeAssistant.Infrastructure.Data.Models;
using HomeAssistant.Middlewares;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<HomeAssistantDbContext>(options =>
options.UseSqlServer(connectionString));

builder.Services.AddSignalR();
builder.Services.AddHostedService<CheckTimerExpirationService>();
builder.Services.AddHostedService<WriteHomeTelemetryDataToDb>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IShoppingListService, ShoppingListService>();
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IWakeOnLanService, WakeOnLanService>();

builder.Services.AddDefaultIdentity<HomeAssistantUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<HomeAssistantDbContext>();

builder.Services.Configure<SecurityStampValidatorOptions>(o =>
o.ValidationInterval = TimeSpan.FromMinutes(1));


builder.Services.AddSingleton<IMongoClient, MongoClient>(s =>
    new MongoClient(MongoClientSettings.FromConnectionString(builder.Configuration.GetConnectionString("MongoUri"))));
builder.Services.AddScoped<IImageService, ImageService>();



builder.Services.AddControllersWithViews(o =>
{
    o.Filters.Add<LogUserInteractionAttribute>();
});

builder.Services.AddMvc(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

builder.Services
    .AddHttpClient<IWeatherService, WeatherService>();

builder.Services
    .AddHttpClient<IHomeTelemetryService, HomeTelemetryService>();

builder.Services
    .AddHttpClient<INASHostService, NASHostService>();

var app = builder.Build();

app.MigrateDatabase<HomeAssistantDbContext>();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseStatusCodePagesWithRedirects("/Home/StatusCode/{0}");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseIsDeletedOrExpiredCheck();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.MapHub<SignalRShoppingCartHub>("/cartHub");

app.MapHub<NotificationsHub>("/notificationHub");

app.MapHub<MessageHub>("/messageHub");

app.MapHub<FridgeHub>("/fridgeHub");

app.MapHub<UsersActiviryHub>("/usersActiviryHub");

app.Run();
