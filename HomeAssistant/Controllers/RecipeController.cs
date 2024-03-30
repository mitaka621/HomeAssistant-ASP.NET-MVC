using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Models.Recipe;
using HomeAssistant.Core.Services;
using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace HomeAssistant.Controllers
{
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
		public IActionResult AddSteps(int recipeId)
		{
			return View();
		}
	}
}
