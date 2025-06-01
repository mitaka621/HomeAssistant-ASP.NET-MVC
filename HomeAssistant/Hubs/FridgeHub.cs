using System.Security.Claims;
using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Models.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HomeAssistant.Hubs
{
    [Authorize(Roles = "StandardUser")]
    public class FridgeHub : Hub
    {
        private readonly IProductService _productService;
        private readonly IHubContext<NotificationsHub> _notificationHubContext;
        private readonly INotificationService _notificationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;

        public FridgeHub(IProductService productService, IHubContext<NotificationsHub> notificationHubContext, INotificationService notificationService, IHttpContextAccessor httpContextAccessor, ILogger<FridgeHub> logger)
        {
            _productService = productService;
            _notificationHubContext = notificationHubContext;
            _notificationService = notificationService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task DecreaseProductQuantity(int productId)
        {
            if (!UserRequestsTracker.HandleConnection(GetUserId()))
            {
                _logger.LogWarning($"User {GetUserId()} doesn't have any more requests for the day");

                await Clients
                    .User(GetUserId())
                    .SendAsync("RequestLimitReached");

                throw new InvalidOperationException();
            }
            await _productService.DecreaseQuantityByOne(productId);

            var product = await _productService.GetProduct(productId);

            var notificationId = await _notificationService.CreateNotificationForAllUsersExceptOne(
                product.Name + " removed from fridge",
                 "Remaining: " + product.Count,
                 GetUserId(),
                 "/Fridge/DecreaseProductQuantity",
            GetUserId());



            _ = Clients
                .AllExcept(Context.ConnectionId)
                .SendAsync("UpdateProductQuantity", productId, false);

            var httpContext = _httpContextAccessor.HttpContext;
            _ = _notificationHubContext.Clients
                .AllExcept(Context.ConnectionId)
                .SendAsync("PushNotfication", new NotificationViewModel()
                {
                    Id = notificationId,
                    CreatedOn = DateTime.Now,
                    Description = "Remaining: " + product.Count,
                    Invoker = new NotificationUserViewModel()
                    {
                        Id = GetUserId(),
                        FirstName = Context.User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty,
                    },
                    Source = "Fridge",
                    Title = product.Name + " removed from fridge"
                });
        }

        public async Task IncreaseProductQuantity(int productId)
        {
            if (!UserRequestsTracker.HandleConnection(GetUserId()))
            {
                _logger.LogWarning($"User {GetUserId()} doesn't have any more requests for the day");

                await Clients
                    .User(GetUserId())
                    .SendAsync("RequestLimitReached");

                throw new InvalidOperationException();
            }

            await _productService.IncreaseQuantityByOne(productId);

            var product = await _productService.GetProduct(productId);

            var notificationId = await _notificationService.CreateNotificationForAllUsersExceptOne(
                product.Name + " added to fridge",
                 "Product Quantity: " + product.Count,
                 GetUserId(),
                 "/Fridge/IncreaseProductQuantity",
            GetUserId());


            _ = Clients
                .AllExcept(Context.ConnectionId)
                .SendAsync("UpdateProductQuantity", productId, true);

            var httpContext = _httpContextAccessor.HttpContext;
            _ = _notificationHubContext.Clients
                .AllExcept(Context.ConnectionId)
                .SendAsync("PushNotfication", new NotificationViewModel()
                {
                    Id = notificationId,
                    CreatedOn = DateTime.Now,
                    Description = "Product Quantity: " + product.Count,
                    Invoker = new NotificationUserViewModel()
                    {
                        Id = GetUserId(),
                        FirstName = Context.User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty,
                    },
                    Source = "Fridge",
                    Title = product.Name + " added to fridge",
                });

        }

        private string GetUserId()
        {
            return Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }
    }

    public static class UserRequestsTracker
    {
        private static readonly List<UserRequestsData> data = new();

        public static bool HandleConnection(string userId)
        {
            if (!data.Any(x => x.UserId == userId))
            {
                data.Add(new UserRequestsData() { UserId = userId });
            }

            var userData = data.First(x => x.UserId == userId);

            if (TimeSpan.FromHours(24) <= DateTime.Now - userData.DateRequestsBegan)
            {
                userData.Reset();
                return true;
            }

            if (userData.RequestsNumber >= userData.MaxRequestThreshold)
            {
                return false;
            }

            userData.RequestsNumber++;

            return true;
        }
    }

    public class UserRequestsData
    {
        public string UserId { set; get; } = string.Empty;

        public int RequestsNumber { get; set; } = 0;

        public int MaxRequestThreshold { get; set; } = 100;

        public DateTime DateRequestsBegan { get; set; } = DateTime.Now;

        public void Reset()
        {
            RequestsNumber = 0;
            DateRequestsBegan = DateTime.Now;
        }
    }

}
