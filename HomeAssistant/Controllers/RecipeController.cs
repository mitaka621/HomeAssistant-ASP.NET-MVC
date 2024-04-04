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

		public async Task<IActionResult> Index(int page = 1)
		{
			return View(await _recipeService.GetAllRecipes(GetUserId(), page));
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
			if (!ModelState.IsValid && s.Duration != null)
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
					return await MoveNextStep(recipeId);
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

		

		[HttpGet]
		public async Task<IActionResult> EditStep(int recipeId, int stepNumber)
		{
			try
			{
				return View(await _recipeService.GetStep(recipeId, stepNumber));
			}
			catch (ArgumentNullException)
			{
				return BadRequest();
			}

		}

		[HttpPost]
		public async Task<IActionResult> EditStep(StepFormViewModel step)
		{
			if (!ModelState.IsValid)
			{
				step.Products = await _recipeService.GetProductsForRecipe(step.RecipeId);
				return View(step);
			}
			try
			{
				await _recipeService.EditStep(step);

				return RedirectToAction(nameof(AddSteps), new { recipeId = step.RecipeId });
			}
			catch (ArgumentNullException)
			{
				return BadRequest();
			}

		}
		[HttpGet]
		public async Task<IActionResult> Delete(int recipeId)
		{
			try
			{
				await _recipeService.DeleteRecipe(recipeId);

				return RedirectToAction(nameof(Index));
			}
			catch (ArgumentNullException)
			{
				return BadRequest();
			}
		}

		[HttpGet]
		public async Task<IActionResult> Edit(int recipeId)
		{
			try
			{
				return View(await _recipeService.GetRecipeFormViewModel(recipeId));
			}
			catch (ArgumentNullException)
			{
				return BadRequest();
			}
		}

		[HttpPost]
		public async Task<IActionResult> Edit(RecipeFormViewModel recipe)
		{
			if (!ModelState.IsValid)
			{
				return View(recipe);
			}
			try
			{
				await _recipeService.EditRecipe(recipe);
				return RedirectToAction(nameof(Index));
			}
			catch (ArgumentNullException)
			{
				return BadRequest();
			}
		}

		[HttpPost]
		public async Task<IActionResult> ChengeStepPos(int recipeId, int oldStepNumber, int newStepNumber)
		{
			try
			{
				await _recipeService.ChangeStepPosition(recipeId, oldStepNumber, newStepNumber);
			}
			catch (ArgumentNullException)
			{
				return BadRequest();
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}

			return RedirectToAction(nameof(AddSteps), new { recipeId = recipeId });



		}

		[HttpPost]
		public async Task<IActionResult> DeleteStep(int recipeId, int stepNumber)
		{
			try
			{
				await _recipeService.DeleteStep(recipeId, stepNumber);
			}
			catch (ArgumentNullException)
			{
				return BadRequest();
			}

			return RedirectToAction(nameof(AddSteps), new { recipeId = recipeId });
		}

		[HttpGet] 
		public async Task<IActionResult> FinishRecipe(int recipeId)
		{
			ViewBag.RecipeId = recipeId;	
			return View(await _recipeService.GetProductsForRecipe(recipeId));
		}

		[HttpPost]
		public async Task<IActionResult> FinishRecipe(int recipeId, IEnumerable<RecipeProductViewModel> products)
		{
			try
			{
				await _recipeService.UpdateProductQuantities(products);
				await _recipeService.DeleteUserRecipeStep(GetUserId(), recipeId);
			}
			catch (ArgumentNullException)
			{
				return BadRequest();
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
			return RedirectToAction(nameof(Index));
		}

		private string GetUserId()
		{
			return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
		}
	}
}
