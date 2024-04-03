﻿using HomeAssistant.Core.Models.Recipe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Contracts
{
	public interface IRecipeService
	{
		Task<int> AddRecipe(RecipeFormViewModel recipe);

		Task<RecipeFormViewModel> GetRecipeFormViewModel(int recipeId);

		Task EditRecipe(RecipeFormViewModel r);

		Task<RecipeDetaislViewModel> GetRecipe(int recipeId);

		Task<Dictionary<int,string>> GetProductsForRecipe(int recipeId);

		Task<StepDetailsViewModel?> GetLastStepDetails(int recipeId);

		Task AddStep(StepFormViewModel step);

		Task<RecipesPaginationViewModel> GetAllRecipes(string userId,int page,int productsOnPage = 10);

		public Task<StepDetailsViewModel?> GetUserStep(string userId, int recipeId);

		public Task MoveNextUserRecipeStep(string userId, int recipeId);

		public Task DeleteUserRecipeStep(string userId, int recipeId);

		public Task<StepFormViewModel> GetStep(int recipeId, int stepNumer);

		public Task EditStep(StepFormViewModel step);

		public Task DeleteRecipe(int recipeId);

		public Task ChangeStepPosition(int recipeId, int oldStepNumber, int newStepNumber);

		public Task DeleteStep(int recipeId, int stepNumber);
    }
}
