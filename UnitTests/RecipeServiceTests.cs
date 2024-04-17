using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Models.Recipe;
using HomeAssistant.Core.Services;
using HomeAssistant.Infrastructure.Data.Models;
using HomeAssistant.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Internal;
using HomeAssistant.Infrastructure.Data.Enums;
using System.Runtime.ExceptionServices;

namespace UnitTests
{
	[TestFixture]
	public class RecipeServiceTests
	{
		private HomeAssistantDbContext _dbContext;
		private IRecipeService _service;
		private Mock<IimageService> _imageService;

		[SetUp]
		public void Setup()
		{
			var options = new DbContextOptionsBuilder<HomeAssistantDbContext>()
					.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
					.Options;
			_dbContext = new HomeAssistantDbContext(options);

			_imageService = new Mock<IimageService>();

			_service = new RecipeService(_dbContext, _imageService.Object);
		}

		[Test]
		public async Task AddRecipe_AddsRecipeAndImage_WhenRecipeImageIsNotNull()
		{
			var viewModel = new RecipeFormViewModel
			{
				Name = "Test Recipe",
				Description = "Test Description",
				RecipeImage = new FormFile(new MemoryStream(), 1, 0, "file", "file.png"),
				SelectedProducts = new List<RecipeProductViewModel> { new RecipeProductViewModel { Id = 1, Quantity = 1 } }
			};

			  
			var result = await _service.AddRecipe(viewModel);

			 
			Assert.Greater(result, 0);
			Assert.AreEqual(1, _dbContext.Recipes.Count());
			Assert.AreEqual("Test Recipe", (await _dbContext.Recipes.FirstAsync(x => x.Id == result)).Name);
		}

		[Test]
		public async Task GetRecipe_ReturnsRecipeDetailsViewModel_WhenRecipeExists()
		{
			int recipeId = 123;

			_dbContext.Recipes.Add(new Recipe()
			{
				Id = recipeId,
				Description = "description",
				Name = "Test",
			});

			await _dbContext.SaveChangesAsync();

			var result = await _service.GetRecipe(recipeId);

			 
			Assert.IsNotNull(result);
			Assert.AreEqual(recipeId, result.Id);
			Assert.AreEqual("Test", result.Name);
			Assert.AreEqual("description", result.Description);
		}

		[Test]
		public async Task GetProductsForRecipe_ReturnsProductViewModels_WhenRecipeExists()
		{
			  
			int recipeId = 123;

			_dbContext.Recipes.Add(new Recipe()
			{
				Id = recipeId,
				Description = "description",
				Name = "Test",
			});

			await _dbContext.SaveChangesAsync();

			var products = new List<RecipeProduct>
			{
				new RecipeProduct {
					RecipeId=recipeId,ProductId = 1, Quantity = 2, Product = new Product { Id = 1, Name = "Product 1", Count = 5 }
				},
				new RecipeProduct {
					RecipeId=recipeId, ProductId = 2, Quantity = 3, Product = new Product { Id = 2, Name = "Product 2", Count = 10 }
				},
			};
			_dbContext.RecipesProducts.AddRange(products);
			await _dbContext.SaveChangesAsync();

			  
			var result = await _service.GetProductsForRecipe(recipeId);

			 
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Count());
			CollectionAssert.AreEquivalent(new string[] { "Product 1", "Product 2" }, result.Select(x => x.Name).ToArray());
		}

		[Test]
		public async Task AddStep_AddsNormalStep_WhenRecipeExists()
		{
			  
			int recipeId = 123;

			_dbContext.Recipes.Add(new Recipe()
			{
				Id = recipeId,
				Description = "description",
				Name = "Test",
			});

			await _dbContext.SaveChangesAsync();


			var stepFormViewModel = new StepFormViewModel
			{
				RecipeId = recipeId,
				Name = "Test Step",
				Description = "Test Description",
				StepType = StepType.NormalStep,
				SelectedProductIds = new int[0],
				StepNumber = 1,
			};

			  
			await _service.AddStep(stepFormViewModel);


			Assert.AreEqual(1, _dbContext.Steps.Count());
			Assert.AreEqual("Test Step", _dbContext.Steps.First().Name);

		}

		[Test]
		public async Task AddStep_AddsNormalStepWithProducts_WhenRecipeExists()
		{
			  
			int recipeId = 123;

			_dbContext.Recipes.Add(new Recipe()
			{
				Id = recipeId,
				Description = "description",
				Name = "Test",
				RecipeProducts = new RecipeProduct[] { new RecipeProduct() { ProductId = 1, Quantity = 2 }, new RecipeProduct() { ProductId = 2, Quantity = 2 } }
			});

			await _dbContext.SaveChangesAsync();


			var stepFormViewModel = new StepFormViewModel
			{
				RecipeId = recipeId,
				Name = "Test Step",
				Description = "Test Description",
				StepType = StepType.NormalStep,
				SelectedProductIds = new int[] { 1, 2 },
				StepNumber = 1,
			};

			  
			await _service.AddStep(stepFormViewModel);

			 

			Assert.AreEqual(1, _dbContext.Steps.Count());
			Assert.AreEqual("Test Step", _dbContext.Steps.First().Name);

			Assert.AreEqual(2, _dbContext.RecipesProductsSteps.Count());
			CollectionAssert.AreEquivalent(new int[] { 1, 2 }, _dbContext.RecipesProducts.Select(x => x.ProductId).ToArray());
		}
		[Test]
		public async Task GetLastStepDetails_GetsLastStep_WhenItExists()
		{
			int recipeId = 123;
			await AddStep_AddsNormalStep_WhenRecipeExists();

			var step = await _service.GetLastStepDetails(recipeId);

			Assert.IsNotNull(step);
			Assert.AreEqual("Test Step", step.Name);
			Assert.AreEqual("Test Description", step.Description);
		}

		[Test]
		public async Task GetAllRecipes_ReturnsRecipesPaginationViewModel_WithStartedRecipes()
		{
			  
			var userId = "testUserId";
			var page = 1;
			var productsOnPage = 10;
			var recipes = new List<Recipe>
			{
				new Recipe { Id = 1, Name = "Recipe 1", Description = "Description 1" },
				new Recipe { Id = 2, Name = "Recipe 2", Description = "Description 2" },
			};

			var steps = new List<Step>
			{
				new Step { RecipeId = 1,StepNumber=1,Name="kartof" },
				new Step { RecipeId = 2,StepNumber=1,Name="qbulka" },
				new Step { RecipeId = 2,StepNumber=2,Name="morkov" },
			};

			var recipeProducts = new List<RecipeProduct>
			{
				new RecipeProduct { RecipeId = 1, ProductId = 1, Quantity = 1 },
				new RecipeProduct { RecipeId = 2, ProductId = 2, Quantity = 2 },
			};

			var usersSteps = new List<UserStep>
			{
				new UserStep { UserId = userId, RecipeId = 1},
				new UserStep { UserId = userId, RecipeId = 2},
			};

			_dbContext.Recipes.AddRange(recipes);
			_dbContext.Steps.AddRange(steps);
			_dbContext.RecipesProducts.AddRange(recipeProducts);
			_dbContext.UsersSteps.AddRange(usersSteps);
			await _dbContext.SaveChangesAsync();

			  
			var result = await _service.GetAllRecipes(userId, page, productsOnPage);

			Assert.AreEqual(1, result.PageCount);
			Assert.AreEqual(1, result.CurrentPage);

			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.StartedRecipes.Count());
			Assert.AreEqual(1, result.StartedRecipes.First().Id);
			Assert.AreEqual("Recipe 1", result.StartedRecipes.First().Name);
		}

		[Test]
		public async Task GetAllRecipes_ReturnsRecipesPaginationViewModel_WithNonStartedRecipes()
		{
			  
			var userId = "testUserId";
			var page = 1;
			var productsOnPage = 10;
			var recipes = new List<Recipe>
			{
				new Recipe { Id = 1, Name = "Recipe 1", Description = "Description 1" },
				new Recipe { Id = 2, Name = "Recipe 2", Description = "Description 2" },
			};
			var steps = new List<Step>
			{
				new Step { RecipeId = 1,StepNumber = 1 },
				new Step { RecipeId = 1,StepNumber = 2 },
			};
			var recipeProducts = new List<RecipeProduct>
			{
				new RecipeProduct { RecipeId = 1, ProductId = 1, Quantity = 1 },
				new RecipeProduct { RecipeId = 2, ProductId = 2, Quantity = 2 },
			};

			_dbContext.Recipes.AddRange(recipes);
			_dbContext.Steps.AddRange(steps);
			_dbContext.RecipesProducts.AddRange(recipeProducts);
			await _dbContext.SaveChangesAsync();

			  
			var result = await _service.GetAllRecipes(userId, page, productsOnPage);

			 
			Assert.IsNotNull(result); // Ensure a non-null result
			Assert.AreEqual(2, result.Recipes.Count());
			Assert.AreEqual(1, result.Recipes.First().Id);
			Assert.AreEqual(true, result.Recipes.First().AnySteps);
			Assert.AreEqual(false, result.Recipes.Skip(1).First().AnySteps);
		}

		[Test]
		public async Task MoveNextUserRecipeStep_AddsUserStep_WhenNoUserStepExists()
		{
			  
			var userId = "testUserId";
			var recipeId = 2;
			var steps = new List<Step>
			{
				new Step { RecipeId = recipeId, StepNumber = 1,Name="Test recipe step 1" },
				new Step { RecipeId = recipeId, StepNumber = 2 },
			};
			_dbContext.Steps.AddRange(steps);
			await _dbContext.SaveChangesAsync();

			  
			await _service.MoveNextUserRecipeStep(userId, recipeId);

			 
			var userStep = await _dbContext.UsersSteps.FirstOrDefaultAsync(x => x.UserId == userId && x.RecipeId == recipeId);
			Assert.IsNotNull(userStep);
			Assert.AreEqual(1, userStep.StepNumber);
			Assert.AreEqual(2, userStep.RecipeId);
		}

		[Test]
		public async Task MoveNextUserRecipeStep_IncrementsStep_WhenUserStepExists()
		{
			  
			var userId = "testUserId";
			var recipeId = 2;

			await MoveNextUserRecipeStep_AddsUserStep_WhenNoUserStepExists();

			  
			await _service.MoveNextUserRecipeStep(userId, recipeId);

			 
			var updatedUserStep = await _dbContext.UsersSteps.FirstOrDefaultAsync(x => x.UserId == userId && x.RecipeId == recipeId);
			Assert.IsNotNull(updatedUserStep);
			Assert.AreEqual(2, updatedUserStep.StepNumber);
			Assert.AreEqual(2, updatedUserStep.RecipeId);
		}

		[Test]
		public async Task MoveNextUserRecipeStep_RemovesUserStep_WhenNoNextStepExists()
		{
			  
			var userId = "testUserId";
			var recipeId = 2;

			await MoveNextUserRecipeStep_IncrementsStep_WhenUserStepExists();

			await _service.MoveNextUserRecipeStep(userId, recipeId);

			 
			var removedUserStep = await _dbContext.UsersSteps.FirstOrDefaultAsync(x => x.UserId == userId && x.RecipeId == recipeId);
			Assert.IsNull(removedUserStep);
		}

		[Test]
		public async Task GetUserStep_ReturnsStepDetailsViewModel_WhenUserStepExists()
		{
			  
			var userId = "testUserId";
			var recipeId = 2;

			await MoveNextUserRecipeStep_AddsUserStep_WhenNoUserStepExists();

			  
			var result = await _service.GetUserStep(userId, recipeId);

			 
			Assert.IsNotNull(result);
			Assert.AreEqual(recipeId, result.RecipeId);
			Assert.AreEqual("Test recipe step 1", result.Name);
			Assert.AreEqual(StepType.NormalStep, result.Type);

		}

		[Test]
		public async Task GetUserStep_ReturnsNull_WhenUserStepDoesNotExist()
		{
			var userId = "testUserId";
			var recipeId = 1;

			var result = await _service.GetUserStep(userId, recipeId);

			Assert.IsNull(result);
		}

		[Test]
		public async Task DeleteUserRecipeStep_RemovesUserStep_WhenUserStepExists()
		{
			var userId = "testUserId";
			var recipeId = 1;
			var userStep = new UserStep { UserId = userId, RecipeId = recipeId };
			_dbContext.UsersSteps.Add(userStep);
			await _dbContext.SaveChangesAsync();

			await _service.DeleteUserRecipeStep(userId, recipeId);

			var deletedUserStep = await _dbContext.UsersSteps.FirstOrDefaultAsync(x => x.UserId == userId && x.RecipeId == recipeId);
			Assert.IsNull(deletedUserStep);
		}

		[Test]
		public async Task GetStep_ReturnsStepFormViewModel_WhenStepExists()
		{
			var recipeId = 1;
			var stepNumber = 1;
			var step = new Step
			{
				RecipeId = recipeId,
				StepNumber = stepNumber,
				Name = "Test Step",
				Description = "Test Description",
				DurationInMin = 10,
				StepType = StepType.NormalStep,
			};
			var recipeProductSteps = new List<RecipeProductStep>
			{
				new RecipeProductStep { RecipeId = recipeId, StepNumber = stepNumber, ProductId = 1 },
				new RecipeProductStep { RecipeId = recipeId, StepNumber = stepNumber, ProductId = 2 },
			};
			var recipeProducts = new List<RecipeProduct>
			{
				new RecipeProduct { RecipeId = recipeId, ProductId = 1, Product = new Product { Name = "Product 1" } },
				new RecipeProduct { RecipeId = recipeId, ProductId = 2, Product = new Product { Name = "Product 2" } },
			};
			_dbContext.Steps.Add(step);
			_dbContext.RecipesProducts.AddRange(recipeProducts);
			_dbContext.RecipesProductsSteps.AddRange(recipeProductSteps);
			await _dbContext.SaveChangesAsync();

			var result = await _service.GetStep(recipeId, stepNumber);

			Assert.IsNotNull(result);
			Assert.AreEqual("Test Step", result.Name);
			Assert.AreEqual("Test Description", result.Description);
			Assert.AreEqual(10, result.Duration);
			Assert.AreEqual(stepNumber, result.StepNumber);
			Assert.AreEqual(StepType.NormalStep, result.StepType);
			Assert.AreEqual(2, result.Products.Count());
			Assert.AreEqual(1, result.Products.First().Id);
			Assert.AreEqual("Product 1", result.Products.First().Name);
			Assert.AreEqual(2, result.Products.Skip(1).First().Id);
			Assert.AreEqual("Product 2", result.Products.Skip(1).First().Name);
		}

		[Test]
		public async Task EditStep_UpdatesStepAndAssociatedProducts_WhenStepExists()
		{
			var recipeId = 1;
			var stepNumber = 1;
			var step = new Step
			{
				RecipeId = recipeId,
				StepNumber = stepNumber,
				Name = "Old Step Name",
				Description = "Old Description",
				DurationInMin = 10,
			};
			var recipeProductStep = new RecipeProductStep { RecipeId = recipeId, StepNumber = stepNumber, ProductId = 1 };
			_dbContext.Steps.Add(step);
			_dbContext.RecipesProductsSteps.Add(recipeProductStep);
			await _dbContext.SaveChangesAsync();

			var viewModel = new StepFormViewModel
			{
				RecipeId = recipeId,
				StepNumber = stepNumber,
				Name = "New Step Name",
				Description = "New Description",
				Duration = 15,
				SelectedProductIds = new int[] { 2, 3 }
			};

			await _service.EditStep(viewModel);

			var updatedStep = await _dbContext.Steps.FirstOrDefaultAsync(x => x.RecipeId == recipeId && x.StepNumber == stepNumber);
			Assert.IsNotNull(updatedStep);
			Assert.AreEqual("New Step Name", updatedStep.Name);
			Assert.AreEqual("New Description", updatedStep.Description);
			Assert.AreEqual(15, updatedStep.DurationInMin);

			var updatedProducts = await _dbContext.RecipesProductsSteps.Where(x => x.RecipeId == recipeId && x.StepNumber == stepNumber).ToListAsync();
			Assert.AreEqual(2, updatedProducts.Count);
			Assert.IsTrue(updatedProducts.Any(x => x.ProductId == 2));
			Assert.IsTrue(updatedProducts.Any(x => x.ProductId == 3));
		}

		[Test]
		public async Task DeleteRecipe_DeletesRecipeAndAssociatedEntities_WhenRecipeExists()
		{
			var recipeId = 1;
			var recipe = new Recipe { Id = recipeId };
			var recipeProduct = new RecipeProduct { RecipeId = recipeId, ProductId = 1 };
			var recipeProductStep = new RecipeProductStep { RecipeId = recipeId, StepNumber = 1, ProductId = 1 };
			var userStep = new UserStep { RecipeId = recipeId };
			_dbContext.Recipes.Add(recipe);
			_dbContext.RecipesProducts.Add(recipeProduct);
			_dbContext.RecipesProductsSteps.Add(recipeProductStep);
			_dbContext.UsersSteps.Add(userStep);
			await _dbContext.SaveChangesAsync();

			await _service.DeleteRecipe(recipeId);

			Assert.AreEqual(0, await _dbContext.Recipes.CountAsync());
			Assert.AreEqual(0, await _dbContext.RecipesProducts.CountAsync());
			Assert.AreEqual(0, await _dbContext.RecipesProductsSteps.CountAsync());
			Assert.AreEqual(0, await _dbContext.UsersSteps.CountAsync());
		}

		[Test]
		public async Task GetRecipeFormViewModel_ReturnsRecipeFormViewModel_WhenRecipeExists()
		{
			var recipeId = 1;
			var recipe = new Recipe { Id = recipeId, Name = "Test Recipe", Description = "Test Description" };
			var recipeProduct = new RecipeProduct { RecipeId = recipeId, ProductId = 1 };
			_dbContext.Recipes.Add(recipe);
			_dbContext.RecipesProducts.Add(recipeProduct);
			await _dbContext.SaveChangesAsync();

			var result = await _service.GetRecipeFormViewModel(recipeId);

			Assert.IsNotNull(result);
			Assert.AreEqual(recipeId, result.Id);
			Assert.AreEqual("Test Recipe", result.Name);
			Assert.AreEqual("Test Description", result.Description);
			// Add more assertions as needed
		}

		[Test]
		public async Task EditRecipe_UpdatesRecipe_WhenRecipeExists()
		{
			var recipeId = 1;
			var recipe = new Recipe { Id = recipeId, Name = "Old Recipe", Description = "Old Description" };
			var recipeProduct = new RecipeProduct { RecipeId = recipeId, ProductId = 1 };
			_dbContext.Recipes.Add(recipe);
			_dbContext.RecipesProducts.Add(recipeProduct);
			await _dbContext.SaveChangesAsync();

			var viewModel = new RecipeFormViewModel
			{
				Id = recipeId,
				Name = "New Recipe",
				Description = "New Description",
				SelectedProducts = new List<RecipeProductViewModel> { new RecipeProductViewModel { Id = 2, Quantity = 1 } }
			};

			await _service.EditRecipe(viewModel);

			var updatedRecipe = await _dbContext.Recipes.FirstOrDefaultAsync(x => x.Id == recipeId);
			Assert.IsNotNull(updatedRecipe);
			Assert.AreEqual("New Recipe", updatedRecipe.Name);
			Assert.AreEqual("New Description", updatedRecipe.Description);

			var updatedRecipeProducts = await _dbContext.RecipesProducts.Where(x => x.RecipeId == recipeId).ToListAsync();
			Assert.AreEqual(1, updatedRecipeProducts.Count);
			Assert.AreEqual(2, updatedRecipeProducts[0].ProductId);
		}

		[Test]
		public async Task ChangeStepPosition_ChangesStepPosition_WhenOldAndNewStepNumbersAreValid()
		{
			var recipeId = 1;
			_dbContext.Recipes.Add(new Recipe()
			{
				Id = recipeId,
				Name = "Title",
			});

			await _dbContext.SaveChangesAsync();

			await _service.AddStep(new StepFormViewModel()
			{
				RecipeId = recipeId,
				Name = "Step1",
				Description = "Description",
			});

			await _service.AddStep(new StepFormViewModel()
			{
				RecipeId = recipeId,
				Name = "Step2",
				Description = "Description",
			});


			Assert.AreEqual("Step1", _dbContext.Steps.First(x => x.RecipeId == recipeId && x.StepNumber == 1).Name);

			Assert.AreEqual("Step2", _dbContext.Steps.First(x => x.RecipeId == recipeId && x.StepNumber == 2).Name);


			await _service.ChangeStepPosition(recipeId, 1, 2);

			 
			var updatedStep1 = await _dbContext.Steps.FirstOrDefaultAsync(x => x.RecipeId == recipeId && x.StepNumber == 1);
			var updatedStep2 = await _dbContext.Steps.FirstOrDefaultAsync(x => x.RecipeId == recipeId && x.StepNumber == 2);
			Assert.IsNotNull(updatedStep1);
			Assert.IsNotNull(updatedStep2);

			Assert.AreEqual("Step2", _dbContext.Steps.First(x => x.RecipeId == recipeId && x.StepNumber == 1).Name);

			Assert.AreEqual("Step1", _dbContext.Steps.First(x => x.RecipeId == recipeId && x.StepNumber == 2).Name);

		}

		[Test]
		public async Task DeleteStep_RemovesStepAndUpdatesStepNumbers_WhenStepExists()
		{
			var recipeId = 1;
			var stepNumber = 2;
			var step = new Step { RecipeId = recipeId, StepNumber = stepNumber };
			var stepProducts = new List<RecipeProductStep>
			{
				new RecipeProductStep { RecipeId = recipeId, StepNumber = stepNumber, ProductId=1 }
			};
			_dbContext.Steps.Add(step);
			_dbContext.RecipesProductsSteps.AddRange(stepProducts);
			await _dbContext.SaveChangesAsync();

			await _service.DeleteStep(recipeId, stepNumber);

			Assert.AreEqual(0, _dbContext.Steps.Count(x => x.RecipeId == recipeId && x.StepNumber == stepNumber));
			Assert.AreEqual(0, _dbContext.RecipesProductsSteps.Count(x => x.RecipeId == recipeId && x.StepNumber == stepNumber));
		}

		[Test]
		public async Task UpdateProductQuantities_DecreasesProductQuantities_WhenProductsExistAndQuantitiesAreValid()
		{
			var productsToUpdate = new List<RecipeProductViewModel>
			{
				new RecipeProductViewModel { Id = 1, Quantity = 3 },
				new RecipeProductViewModel { Id = 2, Quantity = 2 }
			};
			var products = new List<Product>
			{
				new Product { Id = 1, Count = 5 },
				new Product { Id = 2, Count = 4 }
			};
			_dbContext.Products.AddRange(products);
			await _dbContext.SaveChangesAsync();


			await _service.UpdateProductQuantities(productsToUpdate);

			Assert.AreEqual(2, _dbContext.Products.Count());
			Assert.AreEqual(2, _dbContext.Products.First(x => x.Id == 1).Count);
			Assert.AreEqual(2, _dbContext.Products.First(x => x.Id == 2).Count);
		}

		[Test]
		public void UpdateProductQuantities_ThrowsArgumentNullException_WhenProductDoesNotExist()
		{

			var productsToUpdate = new List<RecipeProductViewModel>
			{
				new RecipeProductViewModel { Id = 1, Quantity = 3 },
				new RecipeProductViewModel { Id = 2, Quantity = 2 }
			};

			Assert.ThrowsAsync<ArgumentNullException>(async () => await _service.UpdateProductQuantities(productsToUpdate));
		}

		[Test]
		public async Task UpdateProductQuantities_ThrowsInvalidOperationException_WhenQuantityIsGreaterThanExistingCount()
		{
			var productsToUpdate = new List<RecipeProductViewModel>
			{
				new RecipeProductViewModel { Id = 1, Quantity = 10 }
			};
			var products = new List<Product>
			{
				new Product { Id = 1, Count = 5 }
			};
			_dbContext.Products.AddRange(products);
			await _dbContext.SaveChangesAsync();


			Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.UpdateProductQuantities(productsToUpdate));
		}

		[Test]
		public async Task GetUsersWithExpiredTimers_ReturnsEmptyList_WhenNoExpiredTimersExist()
		{
			int recipeId = 123;

			_dbContext.Recipes.Add(new Recipe
			{
				Id = recipeId,
				Name = "Title",
				Steps = new Step[] { new Step() {RecipeId=recipeId, StepType = StepType.TimerStep, StepNumber = 1 ,DurationInMin=60} }

			});

			var usersSteps = new List<UserStep>
			{
				new UserStep {RecipeId=recipeId, UserId = "user1", StartedOn = DateTime.Now }
			};
			_dbContext.UsersSteps.AddRange(usersSteps);
			await _dbContext.SaveChangesAsync();

			var result = await _service.GetUsersWithExpiredTimers();

			Assert.NotNull(result);
			Assert.IsEmpty(result);
		}
	}
}
