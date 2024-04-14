using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Models.Fridge;
using HomeAssistant.Core.Models.Recipe;
using HomeAssistant.Infrastructure.Data;
using HomeAssistant.Infrastructure.Data.Enums;
using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Core.Operations;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace HomeAssistant.Core.Services
{
	public class RecipeService : IRecipeService
	{
		private readonly HomeAssistantDbContext _dbcontext;
		private readonly IimageService _imageService;

		public RecipeService(HomeAssistantDbContext dbcontext, IimageService imageService)
		{
			_dbcontext = dbcontext;
			_imageService = imageService;
		}

		public async Task<int> AddRecipe(RecipeFormViewModel r)
		{
			Recipe newRecipe = new Recipe()
			{
				Description = r.Description,
				Name = r.Name,
				RecipeProducts = r.SelectedProducts
				.DistinctBy(x => x.Id)
				.Where(x => x.Quantity > 0)
				.Select(x => new RecipeProduct()
				{
					ProductId = x.Id,
					Quantity = x.Quantity,
				}).ToArray()
			};
			_dbcontext.Recipes.Add(newRecipe);
			await _dbcontext.SaveChangesAsync();

			await _imageService.DeleteIfExistsRecipeImg(newRecipe.Id);

			if (r.RecipeImage != null && r.RecipeImage.Length > 0)
			{
				using (var stream = new MemoryStream())
				{
					r.RecipeImage.CopyTo(stream);
					var imageData = stream.ToArray();

					await _imageService.SaveRecipeImage(newRecipe.Id, imageData);
				}

			}

			return newRecipe.Id;
		}

		public async Task<RecipeDetaislViewModel> GetRecipe(int id)
		{
			var tokenSource = new CancellationTokenSource();
			CancellationToken ct = tokenSource.Token;
			Task<byte[]> getImage = _imageService.GetRecipeImage(id, ct);

			var recipe = await _dbcontext.Recipes
				.AsNoTracking()
				.Include(x => x.RecipeProducts)
				.ThenInclude(x => x.Product)
				.Include(x => x.Steps)
				.FirstOrDefaultAsync(x => x.Id == id);

			if (recipe == null)
			{
				tokenSource.Cancel();
				throw new ArgumentNullException(nameof(recipe));
			}

			return new RecipeDetaislViewModel()
			{
				Id = id,
				Description = recipe.Description,
				Name = recipe.Name,
				Photo = await getImage,
				Products = recipe.RecipeProducts
				.Select(x => new RecipeProductViewModel()
				{
					Id=x.ProductId,
					Name = x.Product.Name,
					Quantity = x.Quantity,
				}).ToArray(),
				Steps = recipe.Steps.Select(x => new StepViewModel()
				{
					Description = x.Description,
					Duration = x.DurationInMin,
					Name = x.Name,
					StepNumber = x.StepNumber,
					Type = x.StepType,
				})
			};
		}

		public async Task<IEnumerable<RecipeProductViewModel>> GetProductsForRecipe(int recipeId)
		{
			return await _dbcontext.RecipesProducts
				.AsNoTracking()
				.Where(x => x.RecipeId == recipeId)
				.Select(x => new RecipeProductViewModel()
				{
					Id = x.ProductId,
					Name = x.Product.Name,
					Quantity = x.Quantity,
					AvailableQuantity = x.Product.Count
				}).ToListAsync();
		}

		public async Task AddStep(StepFormViewModel step)
		{
			var recipe = await _dbcontext.Recipes
				.AsNoTracking()
				.FirstOrDefaultAsync(x => x.Id == step.RecipeId);

			var products = await _dbcontext.RecipesProducts
				.AsNoTracking()
				.Where(x => step.SelectedProductIds.Contains(x.ProductId) && x.RecipeId == step.RecipeId)
				.ToListAsync();

			if (recipe == null)
			{
				throw new ArgumentNullException(nameof(recipe));
			}

			if (products.Count != step.SelectedProductIds.Length)
			{
				throw new InvalidOperationException();
			}

			var lastStep = await _dbcontext.Steps
				.AsNoTracking()
				.Where(x => x.RecipeId == step.RecipeId)
				.OrderByDescending(x => x.StepNumber)
				.FirstOrDefaultAsync();

			int newStepNumber = lastStep == null ? 1
				: lastStep.StepNumber + 1;

			_dbcontext.Steps.Add(new Step()
			{
				RecipeId = step.RecipeId,
				StepNumber = newStepNumber,
				Name = step.Name,
				Description = step.Description,
				DurationInMin = step.Duration,
				StepType = step.StepType,
			});

			_dbcontext.RecipesProductsSteps
				.AddRange(step.SelectedProductIds.Select(x => new RecipeProductStep()
				{
					ProductId = x,
					StepNumber = newStepNumber,
					RecipeId = step.RecipeId,
				}).ToArray());

			await _dbcontext.SaveChangesAsync();

		}

		public async Task<StepDetailsViewModel?> GetLastStepDetails(int recipeId)
		{
			var lastStep = await _dbcontext.Steps
				.AsNoTracking()
				.Where(x => x.RecipeId == recipeId)
				.OrderByDescending(x => x.StepNumber)
				.FirstOrDefaultAsync();

			if (lastStep == null)
			{
				return null;
			}

			return new StepDetailsViewModel()
			{
				Description = lastStep.Description,
				Duration = lastStep.DurationInMin,
				Name = lastStep.Name,
				StepNumber = lastStep.StepNumber,
			};
		}

		public async Task<RecipesPaginationViewModel> GetAllRecipes(string userId, int page, int productsOnPage = 10)
		{
			var recipesToReturn = _dbcontext.Recipes.AsNoTracking();
			RecipesPaginationViewModel finalModel = new();

			var startedRecipes = await _dbcontext.UsersSteps
				.AsNoTracking()
				.Where(x => x.UserId == userId)
				.Select(x => new { x.RecipeId, x.StepNumber })
				  .ToDictionaryAsync(x => x.RecipeId, x => x.StepNumber);

			var startedRecipesModel = await recipesToReturn.Where(x => startedRecipes.Keys.Contains(x.Id)).Select(x => new RecipeDetaislViewModel()
			{
				Id = x.Id,
				Description = x.Description,
				Name = x.Name
			})
				.ToListAsync();

			foreach (var item in startedRecipesModel)
			{
				startedRecipesModel
					.First(x => x.Id == item.Id).PercentageCompleted = startedRecipes.ToList()
						.First(d => d.Key == item.Id).Value * 100
						/
						await _dbcontext.Steps
						.Where(s => s.RecipeId == item.Id)
						.CountAsync();

				startedRecipesModel
					.First(x => x.Id == item.Id).Photo = await _imageService.GetRecipeImage(item.Id);

			}

			finalModel.StartedRecipes = startedRecipesModel;

			finalModel.PageCount = (int)Math.Ceiling((await recipesToReturn.CountAsync()) / (double)productsOnPage);

			if (page < 1)
			{
				page = 1;
			}
			else if (page > finalModel.PageCount && finalModel.PageCount != 0)
			{
				page = finalModel.PageCount;
			}

			finalModel.CurrentPage = page;

			recipesToReturn = recipesToReturn
				.Where(x => !startedRecipes.Keys.Contains(x.Id))
				.Skip((page - 1) * productsOnPage)
				.Take(productsOnPage);

			finalModel.Recipes = await recipesToReturn.Select(x => new RecipeDetaislViewModel()
			{
				Id = x.Id,
				Description = x.Description,
				Name = x.Name,
				Products = x.RecipeProducts.Select(x => new RecipeProductViewModel()
				{
					Id = x.ProductId,
					Name = x.Product.Name,
					Quantity = x.Quantity,
				}).ToArray(),
				AnySteps = x.Steps.Any(y => y.RecipeId == x.Id)
			}).ToArrayAsync();

			foreach (var recipe in finalModel.Recipes)
			{
				finalModel.Recipes.First(x => x.Id == recipe.Id).Photo = await _imageService.GetRecipeImage(recipe.Id);

				var prodForRecipe = await _dbcontext.Products
					.Where(x => recipe.Products.Select(x => x.Id).Contains(x.Id))
					.ToListAsync();
				foreach (var product in recipe.Products)
				{
					var currentProd = finalModel.Recipes
						.First(x => x.Id == recipe.Id)
						.Products
						.First(x => x.Id == product.Id);

					currentProd.IsAvailable = prodForRecipe
										.First(x => x.Id == product.Id).Count >= product.Quantity ? true : false;

					currentProd.AvailableQuantity = prodForRecipe
										.First(x => x.Id == product.Id).Count;
				}
			}

			return finalModel;
		}

		public async Task MoveNextUserRecipeStep(string userId, int recipeId)
		{
			var recipeSteps = _dbcontext.Steps
				.AsNoTracking()
				.Where(x => x.RecipeId == recipeId)
				.ToList();
			if (recipeSteps.Count() == 0)
			{
				throw new InvalidOperationException();
			}

			var step = await _dbcontext.UsersSteps
				.FirstOrDefaultAsync(x => x.UserId == userId && x.RecipeId == recipeId);


			if (step == null)
			{
				step = new UserStep()
				{
					RecipeId = recipeId,
					StartedOn = DateTime.Now,
					UserId = userId,
					StepNumber = 1
				};

				_dbcontext.Add(step);

				await _dbcontext.SaveChangesAsync();

				return;
			}

			var nextStep = recipeSteps.FirstOrDefault(x => x.StepNumber == step.StepNumber + 1);

			if (nextStep == null)
			{
				_dbcontext.UsersSteps.Remove(step);
				await _dbcontext.SaveChangesAsync();

				return;
			}

			step.StepNumber++;
			step.StartedOn = DateTime.Now;
			await _dbcontext.SaveChangesAsync();
		}
		public async Task<StepDetailsViewModel?> GetUserStep(string userId, int recipeId)
		{
			var userStep = await _dbcontext.UsersSteps
				.AsNoTracking()
				.Include(x => x.Step)
				.FirstOrDefaultAsync(x => x.UserId == userId && x.RecipeId == recipeId);

			if (userStep == null)
			{
				return null;
			}


			return new StepDetailsViewModel()
			{
				RecipeId = recipeId,
				Description = userStep.Step.Description,
				Name = userStep.Step.Name,
				Duration = userStep.Step.DurationInMin,
				StepNumber = userStep.Step.StepNumber,
				Type = userStep.Step.StepType,
				InitiatedOn = userStep.StartedOn,
				Products = await _dbcontext.RecipesProductsSteps
					.Where(x => x.RecipeId == recipeId && x.StepNumber == userStep.Step.StepNumber)
					.Select(x => x.RecipeProduct.Product.Name)
					.ToListAsync(),
				TotalStepsCount= await _dbcontext.Steps
				.Where(x => x.RecipeId == recipeId)
				.CountAsync()
		};
		}

		public async Task DeleteUserRecipeStep(string userId, int recipeId)
		{
			var step = await _dbcontext.UsersSteps
				.FirstOrDefaultAsync(x => x.UserId == userId && x.RecipeId == recipeId);

			if (step == null)
			{
				throw new ArgumentNullException(nameof(step));
			}

			_dbcontext.UsersSteps.Remove(step);

			await _dbcontext.SaveChangesAsync();
		}

		public async Task<StepFormViewModel> GetStep(int recipeId, int stepNumer)
		{
			var step = await _dbcontext.Steps
				.AsNoTracking()
				.Include(x => x.RecipeProductStep)
				.FirstOrDefaultAsync(x => x.RecipeId == recipeId && x.StepNumber == stepNumer);

			if (step == null)
			{
				throw new ArgumentNullException(nameof(step));
			}
			var model = new StepFormViewModel()
			{
				Description = step.Description,
				Duration = step.DurationInMin,
				StepNumber = step.StepNumber,
				Name = step.Name,
				Products = await _dbcontext.RecipesProducts.Where(x => x.RecipeId == recipeId)
				.Select(t => new RecipeProductViewModel()
				{
					Id = t.ProductId,
					Name = t.Product.Name,
				})
				   .ToListAsync(),
				SelectedProductIds = step.RecipeProductStep.Where(x => x.RecipeId == recipeId && x.StepNumber == stepNumer).Select(x => x.ProductId).ToArray(),
				StepType = step.StepType,
			};


			var totalStepsCount = await _dbcontext.Steps
				.Where(x => x.RecipeId == recipeId)
				.CountAsync();



			return model;
		}

		public async Task EditStep(StepFormViewModel s)
		{
			var step = await _dbcontext.Steps
				.FirstOrDefaultAsync(x => x.RecipeId == s.RecipeId && x.StepNumber == s.StepNumber);

			if (step == null)
			{
				throw new ArgumentNullException(nameof(step));
			}

			step.Name = s.Name;
			step.Description = s.Description;
			step.DurationInMin = s.Duration;
			step.StepNumber = s.StepNumber;

			var oldProdForStep = await _dbcontext.RecipesProductsSteps
				.Where(x => x.RecipeId == s.RecipeId && x.StepNumber == s.StepNumber)
				.ToListAsync();

			_dbcontext.RecipesProductsSteps.RemoveRange(oldProdForStep);
			_dbcontext.RecipesProductsSteps
				.AddRange(s.SelectedProductIds.Select(x => new RecipeProductStep()
				{
					ProductId = x,
					StepNumber = s.StepNumber,
					RecipeId = s.RecipeId,
				}).ToArray());

			await _dbcontext.SaveChangesAsync();
		}

		public async Task DeleteRecipe(int recipeId)
		{
			var recipe = await _dbcontext.Recipes.FirstOrDefaultAsync(x => x.Id == recipeId);

			if (recipe == null)
			{
				throw new ArgumentNullException(nameof(recipe));
			}

			Task deleteImg=_imageService.DeleteIfExistsRecipeImg(recipeId);

			var steps = await _dbcontext.RecipesProductsSteps.Where(x => x.RecipeId == recipeId).ToListAsync();
			var userSteps = await _dbcontext.UsersSteps.Where(x => x.RecipeId == recipeId).ToListAsync();
			var recipeProducts = await _dbcontext.RecipesProducts.Where(x => x.RecipeId == recipeId).ToListAsync();


			_dbcontext.UsersSteps.RemoveRange(userSteps);
			_dbcontext.RecipesProducts.RemoveRange(recipeProducts);
			_dbcontext.RecipesProductsSteps.RemoveRange(steps);
			_dbcontext.Recipes.Remove(recipe);

			_dbcontext.SaveChanges();

			await deleteImg;
        }

		public async Task<RecipeFormViewModel> GetRecipeFormViewModel(int recipeId)
		{
			var recipe = await _dbcontext.Recipes
				.AsNoTracking()
				.Select(x => new RecipeFormViewModel()
				{
					Description = x.Description,
					Id = x.Id,
					Name = x.Name,
				}).FirstOrDefaultAsync(x => x.Id == recipeId);


			if (recipe == null)
			{
				throw new ArgumentNullException(nameof(recipe));
			}

			recipe.RecipeProducts = await GetProductsForRecipe(recipeId);

			return recipe;

		}

		public async Task EditRecipe(RecipeFormViewModel r)
		{
			var recipe = await _dbcontext.Recipes.FirstOrDefaultAsync(x => x.Id == r.Id);

			if (recipe == null)
			{
				throw new ArgumentNullException(nameof(recipe));
			}

			recipe.Name = r.Name;
			recipe.Description = r.Description;
			recipe.Id = r.Id;

			var prodToDelete = await _dbcontext.RecipesProducts.Where(x => x.RecipeId == r.Id).ToListAsync();

			_dbcontext.RecipesProducts.RemoveRange(prodToDelete);

			recipe.RecipeProducts = r.SelectedProducts
				.Select(x => new RecipeProduct()
				{
					ProductId = x.Id,
					Quantity = x.Quantity,
					RecipeId = r.Id,
				}).ToList();

			if (r.RecipeImage != null && r.RecipeImage.Length > 0)
			{
				using (var stream = new MemoryStream())
				{
					r.RecipeImage.CopyTo(stream);
					var imageData = stream.ToArray();

					await _imageService.SaveRecipeImage(r.Id, imageData);
				}

			}

			await _dbcontext.SaveChangesAsync();
		}

		public async Task ChangeStepPosition(int recipeId, int oldStepNumber, int newStepNumber)
		{
			var step = await _dbcontext.Steps
				.AsNoTracking()
				.FirstOrDefaultAsync(x => x.RecipeId == recipeId && x.StepNumber == oldStepNumber);

			if (step == null)
			{
				throw new ArgumentNullException(nameof(step));
			}
			var stepProducts = await _dbcontext.RecipesProductsSteps.Where(x => x.RecipeId == recipeId && x.StepNumber == oldStepNumber).ToListAsync();

			var totalStepsCount = await _dbcontext.Steps.Where(x => x.RecipeId == recipeId).CountAsync();

			if (oldStepNumber == newStepNumber || newStepNumber < 1 || newStepNumber > totalStepsCount)
			{
				throw new InvalidOperationException();
			}

			var step2 = await _dbcontext.Steps
				.AsNoTracking()
				.FirstOrDefaultAsync(x => x.RecipeId == recipeId && x.StepNumber == newStepNumber);

			if (step2 == null)
			{
				throw new ArgumentNullException(nameof(step2));
			}

			var step2Products = await _dbcontext.RecipesProductsSteps.Where(x => x.RecipeId == recipeId && x.StepNumber == newStepNumber).ToListAsync();


			_dbcontext.RecipesProductsSteps.RemoveRange(stepProducts);
			_dbcontext.RecipesProductsSteps.RemoveRange(step2Products);
			_dbcontext.Steps.RemoveRange(step2, step);
			await _dbcontext.SaveChangesAsync();

			step.StepNumber = newStepNumber;
			stepProducts.ForEach(x => x.StepNumber = newStepNumber);

			step2.StepNumber = oldStepNumber;
			step2Products.ForEach(x => x.StepNumber = oldStepNumber);

			_dbcontext.Steps.AddRange(step, step2);
			_dbcontext.RecipesProductsSteps.AddRange(stepProducts);
			_dbcontext.RecipesProductsSteps.AddRange(step2Products);
			await _dbcontext.SaveChangesAsync();
		}

		public async Task DeleteStep(int recipeId, int stepNumber)
		{
			var step = await _dbcontext.Steps
				.AsNoTracking()
				.FirstOrDefaultAsync(x => x.RecipeId == recipeId && x.StepNumber == stepNumber);

			if (step == null)
			{
				throw new ArgumentNullException(nameof(step));
			}

			var stepProducts = await _dbcontext.RecipesProductsSteps.Where(x => x.RecipeId == recipeId && x.StepNumber == stepNumber).ToListAsync();

			var totalStepsCount = await _dbcontext.Steps.Where(x => x.RecipeId == recipeId).CountAsync();

			var steps = await _dbcontext.Steps
				.AsNoTracking()
				.Where(x => x.RecipeId == recipeId && x.StepNumber > step.StepNumber && x.StepNumber <= totalStepsCount)
				.ToListAsync();

			var stepsProducts = await _dbcontext.RecipesProductsSteps
				.Where(x => x.RecipeId == recipeId && x.StepNumber > step.StepNumber && x.StepNumber <= totalStepsCount).ToListAsync();

			_dbcontext.RecipesProductsSteps.RemoveRange(stepProducts);
			_dbcontext.RecipesProductsSteps.RemoveRange(stepsProducts);
			_dbcontext.Steps.Remove(step);
			_dbcontext.Steps.RemoveRange(steps);
			await _dbcontext.SaveChangesAsync();

			steps.ForEach(x => x.StepNumber--);
			stepsProducts.ForEach(x => x.StepNumber--);
			_dbcontext.Steps.AddRange(steps);
			_dbcontext.RecipesProductsSteps.AddRange(stepsProducts);
			await _dbcontext.SaveChangesAsync();
		}

		public async Task UpdateProductQuantities(IEnumerable<RecipeProductViewModel> prodToUpdate)
		{
			var products = await _dbcontext.Products
				.Where(x => prodToUpdate.Select(y => y.Id).Contains(x.Id))
				.ToListAsync();

            foreach (var product in prodToUpdate.DistinctBy(x=>x.Id))
            {
				var currentProd= products.FirstOrDefault(x => x.Id == product.Id);
                if (currentProd==null)
                {
					throw new ArgumentNullException();
                }

                if (currentProd.Count< product.Quantity|| product.Quantity<0)
                {
					throw new InvalidOperationException();
				}

				currentProd.Count-= product.Quantity;

			}
			await _dbcontext.SaveChangesAsync();
        }

		public async Task<IEnumerable<Tuple<string, int, string>>> GetUsersWithExpiredTimers()
		{
			var alreadyNotifiedUsers = (await _dbcontext.NotificationsUsers
				.AsNoTracking()
				.Include(x => x.Notification)
				.Where(x => !x.IsDismissed && x.Notification.InvokerURL.Contains("RecipeStep?recipeId="))
				.ToListAsync())
				.Select(x =>new { recipeId=int.Parse(x.Notification.InvokerURL.Split("=")[1]),userId=x.UserId })
				.ToList();

			return (await _dbcontext.UsersSteps
				.AsNoTracking()
				.Where(x => x.Step.StepType == StepType.TimerStep)
				.Select(x=>new {x.StartedOn, x.Step.DurationInMin,x.UserId,x.Step.Recipe.Name, x.Step.Recipe.Id })
				.ToListAsync())
				.Where(x=> !alreadyNotifiedUsers.Any(y=>y.userId==x.UserId&&y.recipeId==x.Id)&&(DateTime.Now - x.StartedOn).Minutes > x.DurationInMin.Value)
				.Select(x=>Tuple.Create(x.UserId,x.Id, x.Name))
				.ToList();
		}
	}
}
