using System.Security.Claims;
using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Models.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HomeAssistant.Hubs
{
    [Authorize(Roles = "StandardUser")]
    public class MessageHub : Hub
    {
        private readonly IMessageService _messageService;
        private readonly IHubContext<NotificationsHub> _notificationHubContext;
        private readonly INotificationService _notificationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        private static Dictionary<string, DateTime?> connectedClients = new();

        public MessageHub(
            IMessageService messageService,
            IHubContext<NotificationsHub> notificationHubContext,
            INotificationService notificationService,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            _messageService = messageService;
            _notificationHubContext = notificationHubContext;
            _notificationService = notificationService;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public override Task OnConnectedAsync()
        {
            connectedClients[GetUserId()] = null;
            return base.OnConnectedAsync();
        }

        public async Task SendMessage(int chatRoomId, string recipientId, string message)
        {
            if (connectedClients[GetUserId()].HasValue && DateTime.Now - connectedClients[GetUserId()]!.Value < TimeSpan.FromSeconds(1.5))
            {
                throw new InvalidOperationException("Message sent too soon. - " + (DateTime.Now - connectedClients[GetUserId()]!.Value).TotalSeconds);
            }

            connectedClients[GetUserId()] = DateTime.Now;

            await _messageService
                    .SendMessage(chatRoomId, GetUserId(), recipientId, message);

            if (connectedClients.ContainsKey(recipientId))
            {
                await Clients.User(recipientId).SendAsync("LoadMessage", message);
            }
            else
            {
                await _notificationService.PushNotificationForUser(recipientId,
                    "New Message from " + Context.User!.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty,
                    message,
                    $"{_configuration.GetSection("Configuration")["PublicURL"]}/Message/Chat?recipiantId=" + GetUserId(),
                    $"{_configuration.GetSection("Configuration")["PublicURL"]}/svg/message.png"
                    );

                var notificationId = await _notificationService.CreateNotificationForSpecificUser(
                    "Recieved New Message",
                    message,
                    "/Message",
                    recipientId,
                    GetUserId());


                var httpContext = _httpContextAccessor.HttpContext;

                _ = _notificationHubContext.Clients
                    .User(recipientId)
                    .SendAsync("PushNotfication", new NotificationViewModel()
                    {
                        Id = notificationId,
                        CreatedOn = DateTime.Now,
                        Description = message,
                        Invoker = new NotificationUserViewModel()
                        {
                            Id = GetUserId(),
                            FirstName = Context.User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty,
                        },
                        Source = "Fridge",
                        Title = "Recieved New Message"
                    });
            }
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            connectedClients.Remove(GetUserId());
            return base.OnDisconnectedAsync(exception);
        }

        private string GetUserId()
        {
            return Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }
    }
}
