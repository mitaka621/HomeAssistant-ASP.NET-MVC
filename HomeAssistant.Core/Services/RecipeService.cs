using HomeAssistant.Core.Contracts;
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
				.Where(x => x.RecipeId == recipeId)
				.ToDictionaryAsync(t => t.ProductId, t => t.Product.Name);
		}

		public async Task AddStep(int recipeId, int[] productIds, StepFormViewModel step)
		{
			var recipe = await _dbcontext.Recipes
				.AsNoTracking()
				.FirstOrDefaultAsync(x => x.Id == recipeId);

			var products = await _dbcontext.RecipesProducts
				.AsNoTracking()
				.Where(x => productIds.Contains(x.ProductId))
				.ToListAsync();

			if (recipe == null)
			{
				throw new ArgumentNullException(nameof(recipe));
			}

			if (products.Count != productIds.Length)
			{
				throw new InvalidOperationException();
			}

			var lastStep = await _dbcontext.Steps
				.AsNoTracking()
				.Where(x => x.RecipeId == recipeId)
				.OrderByDescending(x => x.StepNumber)
				.FirstOrDefaultAsync();

			int newStepNumber = lastStep == null ? 1
				: lastStep.StepNumber + 1;

			_dbcontext.Steps.Add(new Step()
			{
				RecipeId = recipeId,
				StepNumber = newStepNumber,
				Name = step.Name,
				Description = step.Description,
				DurationInMin = step.Duration
			});

			_dbcontext.RecipesProductsSteps
				.AddRange(productIds.Select(x => new RecipeProductStep()
				{
					ProductId = x,
					StepNumber = newStepNumber,
					RecipeId = recipeId,
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

            if (lastStep==null)
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
	}
}
