using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Models.Recipe;
using HomeAssistant.Infrastructure.Data;
using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;

namespace HomeAssistant.Core.Services
{
	public class RecipeService : IRecipeService
	{
		private readonly HomeAssistantDbContext _dbcontext;
		private readonly IimageService _imageService;

        public RecipeService(HomeAssistantDbContext dbcontext, IimageService imageService)
        {
			_dbcontext=dbcontext;
			_imageService=imageService;
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
	}
}
