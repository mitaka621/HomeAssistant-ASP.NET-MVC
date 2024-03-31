using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Models.Recipe;
using HomeAssistant.Core.Services;
using HomeAssistant.Infrastructure.Data.Enums;
using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeAssistant.Controllers
{
	[Authorize(Roles = "StandardUser")]
	public class RecipeController : Controller
	{
		private readonly IRecipeService _recipeService;
		public RecipeController(IRecipeService recipeService)
		{
			_recipeService = recipeService;
		}

		public async Task<IActionResult> Index(int page=1)
		{
			return View(await _recipeService.GetAllRecipes(page));
		}

		[HttpGet]
		public IActionResult Add()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Add(RecipeFormViewModel r)
		{
			var id = await _recipeService.AddRecipe(r);

			return RedirectToAction(nameof(AddSteps), new { recipeId = id });
		}

		[HttpGet]
		public async Task<IActionResult> AddSteps(int recipeId)
		{
			try
			{
				return View(await _recipeService.GetRecipe(recipeId));
			}
			catch (ArgumentNullException)
			{
				return BadRequest();
			}

		}

		[HttpGet]
		public async Task<IActionResult> AddNormalStep(int recipeId)
		{
			ViewBag.recipeId = recipeId;
			try
			{
				return View(new StepFormViewModel()
				{
					PreviousStep = await _recipeService.GetLastStepDetails(recipeId),
					Products = await _recipeService.GetProductsForRecipe(recipeId)
				});
			}
			catch (ArgumentNullException)
			{
				return BadRequest();
			}
		}

		[HttpGet]
		public async Task<IActionResult> AddTimerStep(int recipeId)
		{
			ViewBag.recipeId = recipeId;
			try
			{
				return View(new StepFormViewModel()
				{
					PreviousStep = await _recipeService.GetLastStepDetails(recipeId),
				});
			}
			catch (ArgumentNullException)
			{
				return BadRequest();
			}
		}

		[HttpPost]
		public async Task<IActionResult> AddNormalStep(StepFormViewModel s)
		{
			//removing validation bc of inaccurate validation bug regarding input from checkbox
			ModelState.Remove("SelectedProductIds");

			if (!ModelState.IsValid)
			{
				s.PreviousStep = await _recipeService.GetLastStepDetails(s.RecipeId);
				s.Products = await _recipeService.GetProductsForRecipe(s.RecipeId);

				return View(s);
			}
			try
			{
				await _recipeService.AddStep(s);
			}
			catch (ArgumentNullException)
			{
				return BadRequest();
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}

			return RedirectToAction(nameof(AddSteps), new { recipeId = s.RecipeId });
		}

		[HttpPost]
		public async Task<IActionResult> AddTimerStep(StepFormViewModel s)
		{
			if (!ModelState.IsValid&&s.Duration!=null)
			{
				s.PreviousStep = await _recipeService.GetLastStepDetails(s.RecipeId);
				return View(s);
			}

			s.StepType = StepType.TimerStep;

			try
			{
				await _recipeService.AddStep(s);
			}
			catch (ArgumentNullException)
			{
				return BadRequest();
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}

			return RedirectToAction(nameof(AddSteps), new { recipeId = s.RecipeId });
		}

		[HttpGet]
		public async Task<IActionResult> RecipeStep(int recipeId)
		{
			try
			{
				var model = await _recipeService.GetUserStep(GetUserId(), recipeId);

				if (model == null)
				{
					model = await _recipeService.MoveNextUserRecipeStep(GetUserId(), recipeId);
				}

				return View(model);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}         
        }

		[HttpPost]
		public async Task<IActionResult> MoveNextStep(int recipeId)
		{
			await _recipeService.MoveNextUserRecipeStep(GetUserId(), recipeId);
			return RedirectToAction(nameof(RecipeStep), new { recipeId });
		}

		private string GetUserId()
		{
			return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
		}
	}
}
