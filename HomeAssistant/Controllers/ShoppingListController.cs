using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Models.Product;
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

            return View(await _shoppingListService.GetProductsByCategory(GetUserId(), page));
		}

		[HttpGet]
		public async Task<IActionResult> ShoppingListCreation()
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
				await _shoppingListService.MarkAsBought(GetUserId(), prodId);
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
				await _shoppingListService.MarkAsUnbought(GetUserId(), prodId);
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
