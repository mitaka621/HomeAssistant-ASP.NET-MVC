using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Models.ShoppingList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeAssistant.Controllers
{
	[Authorize(Roles ="StandardUser")] 
	public class ShoppingListController : Controller
	{
		private readonly IShoppingListService _shoppingListService;

        public ShoppingListController(IShoppingListService shoppingListService)
        {
			_shoppingListService=shoppingListService;
		}

		[HttpGet]
        public async Task< IActionResult> Index()
		{
			var userId = GetUserId();
			if (!await _shoppingListService.IsStarted(userId))
			{
				return RedirectToAction(nameof(ShoppingList));
			}

            if (!await _shoppingListService.AnyUnboughtProducts(userId))
            {
				await _shoppingListService.DeleteShoppingList(userId);
				return RedirectToAction(nameof(ShoppingList));
			}
            return View();
		}

		[HttpGet]
		public async Task<IActionResult> ShoppingList()
		{
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
				return RedirectToAction(nameof(Index));
			}
			catch (ArgumentException)
			{
				return RedirectToAction(nameof(Index));
			}

			return RedirectToAction(nameof(ShoppingList));
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
				return RedirectToAction(nameof(Index));
			}

			return RedirectToAction(nameof(ShoppingList));
		}


		private string GetUserId()
		{
			return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
		}
	}
}
