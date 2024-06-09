using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Models.Notification;
using HomeAssistant.Core.Models.Recipe;
using HomeAssistant.Core.Services;
using HomeAssistant.Hubs;
using HomeAssistant.Infrastructure.Data.Enums;
using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Org.BouncyCastle.Cms;
using System.Security.Claims;

namespace HomeAssistant.Controllers
{
    [Authorize(Roles = "StandardUser")]
    public class RecipeController : Controller
    {
        private readonly IRecipeService _recipeService;
        private readonly IHubContext<NotificationsHub> _notificationHubContext;
        private readonly INotificationService _notificationService;
        private readonly IProductService _productService;

        public RecipeController(IRecipeService recipeService, IHubContext<NotificationsHub> notificationHubContext, INotificationService notificationService, IProductService productService)
        {
            _recipeService = recipeService;
            _notificationHubContext = notificationHubContext;
            _notificationService = notificationService;
			_productService= productService;

		}

        public async Task<IActionResult> Index(int page = 1)
        {
            return View(await _recipeService.GetAllRecipes(GetUserId(), page));
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            ViewBag.Categories = await _productService.GetAllCategories();

			return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(RecipeFormViewModel r)
        {

            var id = await _recipeService.AddRecipe(r);

            var notificationId = await _notificationService.CreateNotificationForAllUsers(
                   "New Recipe Added - " + r.Name,
                    r.Description,
                   HttpContext.Request.Path.ToString(),
                   GetUserId());

            await _notificationHubContext.Clients
                .All
                .SendAsync("PushNotfication", await _notificationService.GetNotification(notificationId));

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
                    Products = await _recipeService.GetProductsForRecipe(recipeId),
                    RecipeId = recipeId
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
                    RecipeId = recipeId
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
                var model = await _recipeService.GetStep(recipeId, stepNumber);
                model.RecipeId = recipeId;
                return View(model);
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
                var recipeToDelete = await _recipeService.GetRecipe(recipeId);

                var notificationId = await _notificationService.CreateNotificationForAllUsers(
                   "Recipe Deleted - " + recipeToDelete.Name,
                          recipeToDelete.Description,
                   HttpContext.Request.Path.ToString(),
                   GetUserId());

                await _notificationHubContext.Clients
                    .All
                    .SendAsync("PushNotfication", new NotificationViewModel()
                    {
                        CreatedOn=DateTime.Now,
                        Description= recipeToDelete.Description,
                        Id=notificationId,
                        Invoker=new NotificationUserViewModel()
                        {
                            Id= GetUserId(),
                            FirstName= User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty,
                            Photo = (byte[])(HttpContext.Items["ProfilePicture"]??new byte[0])
			            },
                        Title = "Recipe Deleted - " + recipeToDelete.Name
					});

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
            var model = await _recipeService.GetProductsForRecipe(recipeId);

            if (!model.Any(x => x.AvailableQuantity != 0))
            {
                return await FinishRecipe(recipeId, new List<RecipeProductViewModel>());
            }
            return View(model);
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
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }

            var recipe = await _recipeService.GetRecipe(recipeId);

            var notificationId = await _notificationService.CreateNotificationForAllUsersExceptOne(
                "Recipe Finished - " + recipe.Name,
                 "Consumed Products:\r\n"+ string.Join("\r\n", products.Select(x=>$"{recipe.Products.First(y=>y.Id==x.Id).Name}({x.Quantity})")),
                 GetUserId(),
                 HttpContext.Request.Path.ToString(),
                 GetUserId());

            await _notificationHubContext.Clients
                .AllExcept(GetUserId())
				.SendAsync("PushNotfication", new NotificationViewModel()
				{
					Id = notificationId,
					CreatedOn = DateTime.Now,
					Description = "Consumed Products:\r\n" + string.Join("\r\n", products.Select(x => $"{recipe.Products.First(y => y.Id == x.Id).Name}({x.Quantity})")),
					Invoker = new NotificationUserViewModel()
					{
						Id = GetUserId(),
						FirstName = User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty,
						Photo = HttpContext.Items["ProfilePicture"] as byte[] ?? new byte[0]
					},
					Source = "/Recipe",
					Title = "Recipe Finished - " + recipe.Name
				});

			await _notificationService.PushNotificationForAllUsersExcept(GetUserId(),
					 $"{User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty} finished a recipe!",
					$"Prepared recipe - {recipe.Name}.\r\nConsumed Products:\r\n" + string.Join("\r\n", products.Select(x => $"{recipe.Products.First(y => y.Id == x.Id).Name}({x.Quantity})")),
					"https://homehub365681.xyz/Recipes",
					"https://homehub365681.xyz/svg/cooking-pot.svg"
					);

			return RedirectToAction(nameof(Index));
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }
    }
}
