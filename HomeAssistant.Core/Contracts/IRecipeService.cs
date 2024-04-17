using HomeAssistant.Core.Models.Recipe;

namespace HomeAssistant.Core.Contracts
{
	public interface IRecipeService
	{
		Task<int> AddRecipe(RecipeFormViewModel recipe);

		Task<RecipeFormViewModel> GetRecipeFormViewModel(int recipeId);

		Task EditRecipe(RecipeFormViewModel r);

		Task<RecipeDetaislViewModel> GetRecipe(int recipeId);

		Task<IEnumerable<RecipeProductViewModel>> GetProductsForRecipe(int recipeId);

		Task<StepDetailsViewModel?> GetLastStepDetails(int recipeId);

		Task AddStep(StepFormViewModel step);

		Task<RecipesPaginationViewModel> GetAllRecipes(string userId,int page,int productsOnPage = 10);

		public Task<StepDetailsViewModel?> GetUserStep(string userId, int recipeId);

		public Task<IEnumerable<Tuple<string,int, string>>> GetUsersWithExpiredTimers();

		public Task MoveNextUserRecipeStep(string userId, int recipeId);

		public Task DeleteUserRecipeStep(string userId, int recipeId);

		public Task<StepFormViewModel> GetStep(int recipeId, int stepNumer);

		public Task EditStep(StepFormViewModel step);

		public Task DeleteRecipe(int recipeId);

		public Task ChangeStepPosition(int recipeId, int oldStepNumber, int newStepNumber);

		public Task DeleteStep(int recipeId, int stepNumber);

		public Task UpdateProductQuantities( IEnumerable<RecipeProductViewModel> prodToUpdate);
    }
}
