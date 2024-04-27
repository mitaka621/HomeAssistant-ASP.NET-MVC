using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Models.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Security.Claims;
using static MongoDB.Driver.WriteConcern;

namespace HomeAssistant.Hubs
{
	[Authorize(Roles = "StandardUser")]
	public class FridgeHub : Hub
	{
		private readonly IProductService _productService;
		private readonly IHubContext<NotificationsHub> _notificationHubContext;
		private readonly INotificationService _notificationService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public FridgeHub(IProductService productService, IHubContext<NotificationsHub> notificationHubContext, INotificationService notificationService, IHttpContextAccessor httpContextAccessor)
		{
			_productService = productService;
			_notificationHubContext = notificationHubContext;
			_notificationService = notificationService;
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task DecreaseProductQuantity(int productId)
		{
			
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
						Photo = httpContext.Items["ProfilePicture"] as byte[] ?? new byte[0]
					},
					Source= "Fridge",
					Title= product.Name + " removed from fridge"
				});
		}

		public async Task IncreaseProductQuantity(int productId)
		{

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
						Photo= httpContext.Items["ProfilePicture"] as byte[]??new byte[0]
					},
					Source = "Fridge",
					Title =product.Name + " added to fridge",
				});
			
		}

		private string GetUserId()
		{
			return Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
		}
	}
}
