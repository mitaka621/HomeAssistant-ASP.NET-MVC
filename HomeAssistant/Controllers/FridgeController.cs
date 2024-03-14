using HomeAssistant.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeAssistant.Controllers
{
	[Authorize(Roles ="StandardUser")]
	public class FridgeController : Controller
	{
        private readonly IProductService _productService;

        public FridgeController(IProductService productService)
        {
			_productService=productService;

		}

		[HttpGet]
        public async Task<IActionResult> Index
			(string? ToastTitle=null,
			string? ToastMessage = null,
			bool isAvailable = true,
			int? categoryId=null
			)
		{
			ViewBag.ToastTitle = ToastTitle;
			ViewBag.ToastMessage = ToastMessage;
			ViewBag.Available = isAvailable;

			var categories= await _productService.GetAllCategories();
			ViewBag.Categories= categories;

            if (categories.Any(x=>x.Id==categoryId))
            {
				ViewBag.CategoryId = categoryId;
			}    
			
            
            return View(await _productService.GetProducts(isAvailable,categoryId));
		}

		[HttpPost]
		public async Task<IActionResult> DecreaseProductQuantity(int productId)
		{
			try
			{
				await _productService.DecreaseQuantityByOne(productId);
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

			return RedirectToAction(nameof(Index), new
			{
				ToastTitle = "Success",
				ToastMessage = "Product quantity decreased!"
			});
		}

		[HttpPost]
		public async Task<IActionResult> IncreaseProductQuantity(int productId)
		{
			try
			{
				await _productService.IncreaseQuantityByOne(productId);
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

			return RedirectToAction(nameof(Index), new
			{
				ToastTitle = "Success",
				ToastMessage = "Product quantity increased!"
			});
		}

		[HttpGet]
		public async Task<IActionResult> ProductSearch(string keyphrase)
		{
			var products=await _productService.SearchProducts(keyphrase);

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
	}
}
