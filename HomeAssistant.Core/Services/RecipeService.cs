using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Models.Fridge;
using HomeAssistant.Core.Models.Recipe;
using HomeAssistant.Infrastructure.Data;
using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Core.Operations;
using System.Data;
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
				RecipeProducts = r.ProductsIds.Select(x => new RecipeProduct()
				{
					ProductId = x
				}).ToArray()
			};
			_dbcontext.Recipes.Add(newRecipe);
			await _dbcontext.SaveChangesAsync();

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
				Products = recipe.RecipeProducts.Select(x => x.Product.Name).ToArray(),
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

		public async Task<Dictionary<int, string>> GetProductsForRecipe(int recipeId)
		{
			return await _dbcontext.RecipesProducts
				.AsNoTracking()
				.Include(x => x.Product)
				.Where(x => x.RecipeId == recipeId)
				.ToDictionaryAsync(t => t.ProductId, t => t.Product.Name);
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

		public async Task<RecipesPaginationViewModel> GetAllRecipes(string userId,int page, int productsOnPage = 10 )
		{
			var recipesToReturn = _dbcontext.Recipes.AsNoTracking();
			RecipesPaginationViewModel finalModel = new();

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
				.Skip((page - 1) * productsOnPage)
				.Take(productsOnPage);

			finalModel.Recipes = await recipesToReturn.Select(x => new RecipeDetaislViewModel()
			{
				Id = x.Id,
				Description = x.Description,
				Name = x.Name,
				Products = x.RecipeProducts.Where(x => x.Product.Count > 0).Select(x => x.Product.Name).ToArray(),
				ProductsNotAvailable = x.RecipeProducts.Where(x => x.Product.Count == 0).Select(x => x.Product.Name).ToArray(),
				AnySteps = x.Steps.Any(y => y.RecipeId == x.Id)
			}).ToArrayAsync();

			foreach (var item in finalModel.Recipes)
			{
				finalModel.Recipes.First(x => x.Id == item.Id).Photo = await _imageService.GetRecipeImage(item.Id);
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

			var lastStep = await _dbcontext.Steps
				.Where(x => x.RecipeId == recipeId)
				.OrderByDescending(x => x.StepNumber)
				.FirstAsync();


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
				IsLast = lastStep.StepNumber == userStep.StepNumber ? true : false
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
	}
}
