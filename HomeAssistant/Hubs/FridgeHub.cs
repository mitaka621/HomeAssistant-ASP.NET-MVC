using HomeAssistant.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
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

		public FridgeHub(IProductService productService, IHubContext<NotificationsHub> notificationHubContext, INotificationService notificationService)
		{
			_productService = productService;
			_notificationHubContext = notificationHubContext;
			_notificationService = notificationService;
		}

		public async Task DecreaseProductQuantity(int productId)
		{
			var newCount=await _productService.DecreaseQuantityByOne(productId);

			var product = await _productService.GetProduct(productId);

			var notificationId = await _notificationService.CreateNotificationForAllUsersExceptOne(
				product.Name + " removed from fridge",
				 "Remaining: " + product.Count,
				 GetUserId(),
				 "/Fridge",
			GetUserId());

			await Clients
				.All
				.SendAsync("UpdateProductQuantity", productId, newCount);

			await _notificationHubContext.Clients
				.AllExcept(GetUserId())
				.SendAsync("PushNotfication", await _notificationService.GetNotification(notificationId));

		}

		public async Task IncreaseProductQuantity(int productId)
		{
			
			var newCount=await _productService.IncreaseQuantityByOne(productId);

			var product = await _productService.GetProduct(productId);

			var notificationId = await _notificationService.CreateNotificationForAllUsersExceptOne(
				product.Name + " added to fridge",
				 "Product Quantity: " + product.Count,
				 GetUserId(),
				 "/Fridge",
			GetUserId());


			await Clients
			.All
				.SendAsync("UpdateProductQuantity", productId, newCount);

			await _notificationHubContext.Clients
				.AllExcept(GetUserId())
				.SendAsync("PushNotfication", await _notificationService.GetNotification(notificationId));
		}

		private string GetUserId()
		{
			return Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
		}
	}
}
