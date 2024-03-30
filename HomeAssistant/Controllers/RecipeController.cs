using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Models.Recipe;
using HomeAssistant.Core.Services;
using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeAssistant.Controllers
{
	[Authorize(Roles ="StandardUser")]
	public class RecipeController : Controller
	{
		private readonly IRecipeService _recipeService;
		public RecipeController(IRecipeService recipeService)
		{
			_recipeService = recipeService;
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public IActionResult Add()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Add(RecipeFormViewModel r)
		{
			var id=await _recipeService.AddRecipe(r);

			return RedirectToAction(nameof(AddSteps),new { recipeId=id });
		}

		[HttpGet]
		public async Task< IActionResult> AddSteps(int recipeId)
		{
			return View(await _recipeService.GetRecipe(recipeId));
		}

		[HttpGet]
		public async Task<IActionResult> AddNormalStep(int recipeId)
		{
			ViewBag.recipeId=recipeId;
			ViewBag.PreviousStep=await _recipeService.GetLastStepDetails(recipeId);
			ViewBag.Products = await _recipeService.GetProductsForRecipe(recipeId);
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> AddNormalStep(int recipeId,int[] productIds, StepFormViewModel s)
		{
			await _recipeService.AddStep(recipeId, productIds, s);
			return RedirectToAction(nameof(Index));
		}
	}
}
