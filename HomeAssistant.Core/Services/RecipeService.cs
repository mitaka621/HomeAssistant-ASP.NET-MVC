using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Models.Recipe;
using HomeAssistant.Infrastructure.Data;

namespace HomeAssistant.Core.Services
{
	public class RecipeService : IRecipeService
	{
		private readonly HomeAssistantDbContext _dbcontext;
        public RecipeService(HomeAssistantDbContext dbcontext)
        {
			_dbcontext=dbcontext;

		}
        public Task AddRecipe(RecipeFormViewModel r)
		{
			throw new NotImplementedException();
		}
	}
}
