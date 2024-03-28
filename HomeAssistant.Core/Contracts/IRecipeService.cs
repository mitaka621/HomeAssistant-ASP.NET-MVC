using HomeAssistant.Core.Models.Recipe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Contracts
{
	public interface IRecipeService
	{
		Task AddRecipe(RecipeFormViewModel recipe);
	}
}
