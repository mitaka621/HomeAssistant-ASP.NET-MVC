namespace HomeAssistant.Core.Models.Recipe
{
	public class RecipesPaginationViewModel
	{
		public int PageCount { get; set; }

		public int CurrentPage { get; set; }

		public IEnumerable<RecipeDetaislViewModel> Recipes { get; set; } = new List<RecipeDetaislViewModel>();

		public IEnumerable<RecipeDetaislViewModel> StartedRecipes { get; set; } = new List<RecipeDetaislViewModel>();
	}
}
