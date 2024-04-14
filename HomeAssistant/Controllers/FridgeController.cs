using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Enums;
using HomeAssistant.Core.Models.Product;
using HomeAssistant.Core.Services;
using HomeAssistant.Hubs;
using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace HomeAssistant.Controllers
{
    [Authorize(Roles = "StandardUser")]
	public class FridgeController : Controller
	{
		private readonly IProductService _productService;
        private readonly IHubContext<NotificationsHub> _notificationHubContext;
        private readonly INotificationService _notificationService;

        public FridgeController(IProductService productService, IHubContext<NotificationsHub> notificationHubContext, INotificationService notificationService)
		{
			_productService = productService;
            _notificationHubContext= notificationHubContext;
			_notificationService= notificationService;

        }

		[HttpGet]
		public async Task<IActionResult> Index
			(string? ToastTitle = null,
			string? ToastMessage = null,
			bool isAvailable = true,
			int? categoryId = null,
			string orderBy="0",
			int page=1
			)
		{
			OrderBy parsedEnum;

			if (!Enum.TryParse<OrderBy>(orderBy, true, out parsedEnum))
			{
				parsedEnum = OrderBy.Recent;
			}
			
			var fridgeViewModel = await _productService.GetProducts(isAvailable, categoryId, parsedEnum, page);

			fridgeViewModel.LatestFridgeActivity = await _notificationService.GetTop20ProductRelatedNotification();


            ViewBag.OrderBy = parsedEnum;
			ViewBag.ToastTitle = ToastTitle;
			ViewBag.ToastMessage = ToastMessage;
			ViewBag.Available = isAvailable;

			var categories = await _productService.GetAllCategories();
			ViewBag.Categories = categories;

			if (categories.Any(x => x.Id == categoryId))
			{
				ViewBag.CategoryId = categoryId;
			}		
			return View(fridgeViewModel);
		}

		[HttpPost]
		public async Task<IActionResult> DecreaseProductQuantity(int productId)
        {
            try
            {
                await _productService.DecreaseQuantityByOne(productId);

			
                var product = await _productService.GetProduct(productId);

                var notificationId = await _notificationService.CreateNotificationForAllUsers(
                    product.Name+ " removed from fridge",
                     "Remaining: "+ product.Count,
                     HttpContext.Request.Path.ToString(),
                GetUserId());

                _=_notificationHubContext.Clients
                    .All
                    .SendAsync("PushNotfication", await _notificationService.GetNotification(notificationId));
            }
			catch (ArgumentNullException)
			{
				return RedirectToAction(nameof(Index), new
				{
					ToastTitle = "Error",
					ToastMessage = "Internal server error!"
				});
			}
			catch (InvalidOperationException)
			{
				return RedirectToAction(nameof(Index), new
				{
					ToastTitle = "Error",
					ToastMessage = "You can't decrease product quantity less than 0!",
				});
			}

			return Redirect(Request.Headers["Referer"].ToString());
		}

		[HttpPost]
		public async Task<IActionResult> IncreaseProductQuantity(int productId)
		{
			try
			{
				await _productService.IncreaseQuantityByOne(productId);

                var product = await _productService.GetProduct(productId);

                var notificationId = await _notificationService.CreateNotificationForAllUsers(
                    product.Name + " added to fridge",
                     "Product Quantity: " + product.Count,
                     HttpContext.Request.Path.ToString(),
                GetUserId());

               _=_notificationHubContext.Clients
                    .All
                    .SendAsync("PushNotfication", await _notificationService.GetNotification(notificationId));
            }
			catch (ArgumentNullException)
			{
				return RedirectToAction(nameof(Index), new
				{
					ToastTitle = "Error",
					ToastMessage = "Internal server error!"
				});
			}
			catch (InvalidOperationException)
			{
				return RedirectToAction(nameof(Index), new
				{
					ToastTitle = "Error",
					ToastMessage = "Internal server error!"
				});
			}

			return Redirect(Request.Headers["Referer"].ToString());
		}

		[HttpGet]
		public async Task<IActionResult> ProductSearch(string keyphrase)
		{
			var products = await _productService.SearchProducts(keyphrase);

			return Json(products);
		}

		[HttpGet]
		public async Task<IActionResult> EditProduct(int id)
		{
			try
			{
				return View(await _productService.GetProduct(id));
			}
			catch (ArgumentNullException)
			{
				return RedirectToAction(nameof(Index), new
				{
					ToastTitle = "Error",
					ToastMessage = "Internal server error!"
				});
			}
		}
		[HttpPost]
		public async Task<IActionResult> EditProduct(ProductFormViewModel product)
		{
            if (!ModelState.IsValid)
            {
				return View(product);
			}
            try
			{
				await _productService.EditProduct(GetUserId(), product);
			}
			catch (ArgumentNullException)
			{
				return RedirectToAction(nameof(Index), new
				{
					ToastTitle = "Error",
					ToastMessage = "Internal server error!"
				});
			}

			return RedirectToAction(nameof(Index), new
			{
				ToastTitle = "Success",
				ToastMessage = "Product added!"
			});
		}

		[HttpGet]
		public async Task<IActionResult> AddProduct(string prodName = "")
		{
			ViewBag.ProductName = prodName;
			return View(new ProductFormViewModel()
			{
				AllCategories = await _productService.GetAllCategories()
			});
		}

		[HttpPost]
		public async Task<IActionResult> AddProduct(ProductFormViewModel product)
		{
			if (!ModelState.IsValid)
			{
				return View(product);
			}

			await _productService.AddProduct(GetUserId(), product);

			return RedirectToAction(nameof(Index), new
			{
				ToastTitle = "Success",
				ToastMessage = "Product added!"
			});
		}

		[HttpPost]
		public async Task<IActionResult> DeleteProduct(int productId)
		{
			try
			{
				await _productService.DeleteProduct(productId);
			}
			catch (ArgumentNullException)
			{
				return RedirectToAction(nameof(Index), new
				{
					ToastTitle = "Error",
					ToastMessage = "Internal server error!"
				});
			}
			
			return RedirectToAction(nameof(Index), new
			{
				ToastTitle = "Success",
				ToastMessage = "Product deleted!"
			});
		}


		private string GetUserId()
		{
			return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
		}
	}
}
