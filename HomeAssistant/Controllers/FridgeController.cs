using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Enums;
using HomeAssistant.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeAssistant.Controllers
{
	[Authorize(Roles = "StandardUser")]
	public class FridgeController : Controller
	{
		private readonly IProductService _productService;

		public FridgeController(IProductService productService)
		{
			_productService = productService;

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
			ViewBag.ToastTitle = ToastTitle;
			ViewBag.ToastMessage = ToastMessage;
			ViewBag.Available = isAvailable;
			ViewBag.NumberPages = await _productService.GetNumberPages();

            if (page<1)
            {
				page = 1;
			}

            if (page > (int)ViewBag.NumberPages&& ViewBag.NumberPages!=0)
            {
				page= (int)ViewBag.NumberPages;
			}
            ViewBag.CurrentPage = page;

			var categories = await _productService.GetAllCategories();
			ViewBag.Categories = categories;

			if (categories.Any(x => x.Id == categoryId))
			{
				ViewBag.CategoryId = categoryId;
			}

			OrderBy parsedEnum;

			if (!Enum.TryParse<OrderBy>(orderBy, true, out parsedEnum))
			{
				parsedEnum = OrderBy.Recent;
			}

			ViewBag.OrderBy= parsedEnum;

			return View(await _productService.GetProducts(isAvailable, categoryId, parsedEnum,page));
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

			return Redirect(Request.Headers["Referer"].ToString());
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
				ToastMessage = "Product added!"
			});
		}


		private string GetUserId()
		{
			return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
		}
	}
}
