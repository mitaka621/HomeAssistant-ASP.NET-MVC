using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Models.Product;
using HomeAssistant.Core.Models.ShoppingList;
using HomeAssistant.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace HomeAssistant.Controllers
{
	[Authorize(Roles ="StandardUser")] 
	public class ShoppingListController : Controller
	{
		private readonly IShoppingListService _shoppingListService;
		private readonly IHubContext<SignalRShoppingCartHub> _hubContext;

		public ShoppingListController(IShoppingListService shoppingListService, IHubContext<SignalRShoppingCartHub> hubContext)
        {
			_shoppingListService=shoppingListService;
			_hubContext=hubContext;
		}

		[HttpGet]
		public async Task<IActionResult> Index(int page = 1)
		{
			var userId = GetUserId();
			if (!await _shoppingListService.IsStarted(userId))
			{
				return RedirectToAction(nameof(ShoppingListCreation));
			}

            if (await _shoppingListService.IsFinished(userId))
            {
				await _shoppingListService.DeleteShoppingList(userId);
				return RedirectToAction(nameof(ShoppingListCreation));
			}

			ViewBag.ActiveShoppingLists=await _shoppingListService.GetTop20StartedShoppingListsExceptCurrentUser(userId);
            return View(await _shoppingListService.GetProductsByCategory(userId, page));
		}

		[HttpGet]
		public async Task<IActionResult> ShoppingListCreation()
		{
			if (await _shoppingListService.IsStarted(GetUserId()))
			{
				return RedirectToAction(nameof(Index));
			}

			ViewBag.ActiveShoppingLists = await _shoppingListService
				.GetTop20StartedShoppingListsExceptCurrentUser(GetUserId());

			return View(await _shoppingListService.GetShoppingList(GetUserId()));
		}

		[HttpPost]
		public async Task<IActionResult> Product(ShoppingListProductViewModel product)
		{
            if (!ModelState.IsValid)
            {
				RedirectToAction(nameof(Index));
            }
			try
			{
				await _shoppingListService.AddProductToShoppingList(GetUserId(), product);
			}
			catch (ArgumentNullException)
			{
				return BadRequest();
			}
			catch (ArgumentException)
			{
				return BadRequest();
			}

			return RedirectToAction(nameof(ShoppingListCreation));
		}

		[HttpPost]
		public async Task<IActionResult> Delete(int prodId)
		{
			try
			{
				await _shoppingListService.DeleteProductFromShoppingList(GetUserId(), prodId);
			}
			catch (ArgumentNullException)
			{
				return BadRequest();
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		public async Task<IActionResult> NewProduct(ShoppingListProductViewModel product)
		{
			if (!ModelState.IsValid)
			{
				RedirectToAction(nameof(Index));
			}
			try
			{
				await _shoppingListService.AddNewProductToFridgeAndShoppingList(GetUserId(),product);
			}
			catch (ArgumentNullException)
			{
				return BadRequest();
			}
			catch (ArgumentException)
			{
				return BadRequest();
			}

			return RedirectToAction(nameof(ShoppingListCreation	));
		}

        [HttpPost]
        public async Task<IActionResult> StartShopping()
		{
			try
			{
				await _shoppingListService.StartShopping(GetUserId());
			}
			catch (ArgumentNullException)
			{
				return BadRequest();
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		public async Task<IActionResult> CancelShopping()
		{
			try
			{
				await _shoppingListService.CancelShopping(GetUserId());
			}
			catch (ArgumentNullException)
			{
				return BadRequest();
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		public async Task<IActionResult> BuyProduct(int prodId)
		{
			try
			{
				var userId = GetUserId();
				await _shoppingListService.MarkAsBought(userId, prodId);
				await _hubContext.Clients.All
					.SendAsync("GetShoppingCartUpdate", userId, await _shoppingListService.GetShoppingListProgress(userId));
			}
			catch (ArgumentNullException)
			{
				return BadRequest();
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		public async Task<IActionResult> RestoreProduct(int prodId)
		{
			try
			{
				var userId = GetUserId();
				await _shoppingListService.MarkAsUnbought(userId, prodId);
				await _hubContext.Clients.All
					.SendAsync("GetShoppingCartUpdate", userId, await _shoppingListService.GetShoppingListProgress(userId));
			}
			catch (ArgumentNullException)
			{
				return BadRequest();
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		public async Task<IActionResult> FinishShoppingList()
		{
			try
			{
				await _shoppingListService.SaveBoughtProducts(GetUserId());	
			}
			catch (ArgumentNullException)
			{
				return BadRequest();
			}

			return RedirectToAction("Index", "Fridge");
		}


		private string GetUserId()
		{
			return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
		}
	}
}
