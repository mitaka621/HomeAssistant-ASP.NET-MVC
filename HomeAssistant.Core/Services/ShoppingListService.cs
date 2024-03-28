using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Enums;
using HomeAssistant.Core.Models.Product;
using HomeAssistant.Core.Models.ShoppingList;
using HomeAssistant.Infrastructure.Data;
using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeAssistant.Core.Services
{
	public class ShoppingListService : IShoppingListService
	{
		private readonly HomeAssistantDbContext _dbcontext;
		private readonly IProductService _productService;
		private readonly IimageService _ImageService;
		public ShoppingListService(HomeAssistantDbContext dbContext, IProductService productService, IimageService ImageService)
		{
			_dbcontext = dbContext;
			_productService = productService;
			_ImageService = ImageService;
		}

		public async Task<bool> IsStarted(string userId)
		{
			var sl = await _dbcontext.ShoppingLists
				.AsNoTracking()
				.FirstOrDefaultAsync(x => x.UserId == userId);

			if (sl == null)
			{
				return false;
			}

			return sl.IsStarted;
		}

		public async Task<bool> IsFinished(string userId)
		{
			var sl = await _dbcontext
			.ShoppingLists
			.FirstOrDefaultAsync(x => x.UserId == userId);

			if (sl == null)
			{
				throw new ArgumentNullException(nameof(sl));
			}

			return sl.IsFinished;
		}

		public async Task<ShoppingListViewModel> GetShoppingList(string userId)
		{
			var sl = await _dbcontext.ShoppingLists
				.AsNoTracking()
				.FirstOrDefaultAsync(x => x.UserId == userId);

			if (sl == null)
			{
				await _dbcontext.ShoppingLists.AddAsync(new ShoppingList()
				{
					UserId = userId
				});

				await _dbcontext.SaveChangesAsync();

				return new ShoppingListViewModel
				{
					OutOfStockProducts = (await _productService.GetProducts(false, null, OrderBy.Oldest, 0)).Products,
					AllCategories = await _productService.GetAllCategories()
				};
			}

			var products = await _dbcontext.ShoppingListsProducts
				.Where(x => x.ShoppingListId == userId)
				.Select(x => new ShoppingListProductViewModel()
				{
					Id = x.ProductId,
					Name = x.Product.Name,
					CategoryName = x.Product.Category.Name,
					Price = x.StorePrice,
					QuantityToBuy = x.QuantityToBuy,
				})
				.ToListAsync();

			return new ShoppingListViewModel()
			{
				OutOfStockProducts = (await _productService.GetProducts(false, null, OrderBy.Oldest, 0))
					.Products.Where(x => !products.Any(y => y.Id == x.Id)),
				Products = products,
				AllCategories = await _productService.GetAllCategories()
			};
		}

		public async Task DeleteShoppingList(string userId)
		{
			var sl = await _dbcontext.ShoppingLists
				.FirstOrDefaultAsync(x => x.UserId == userId);

			if (sl == null)
			{
				throw new ArgumentNullException();
			}

			_dbcontext.ShoppingLists.Remove(sl);

			await _dbcontext.SaveChangesAsync();
		}

		public async Task AddProductToShoppingList(string userId, ShoppingListProductViewModel product)
		{
			var p = await _dbcontext.Products
				.AsNoTracking()
				.FirstOrDefaultAsync(x => product.Id == x.Id);

			if (p == null)
			{
				throw new ArgumentNullException();
			}

			var record = await _dbcontext
				.ShoppingListsProducts
				.FirstOrDefaultAsync(x => x.ProductId == product.Id && x.ShoppingListId == userId);
			if (record != null)
			{
				throw new ArgumentException();
			}

			await _dbcontext.ShoppingListsProducts.AddAsync(new ShoppingListProduct
			{
				ProductId = product.Id,
				ShoppingListId = userId,
				StorePrice = product.Price,
				QuantityToBuy = product.QuantityToBuy,
			});

			await _dbcontext.SaveChangesAsync();

		}

		public async Task DeleteProductFromShoppingList(string userId, int productId)
		{
			var record = await _dbcontext.ShoppingListsProducts
				.FirstOrDefaultAsync(x => x.ShoppingListId == userId && x.ProductId == productId);

			if (record == null)
			{
				throw new ArgumentNullException();
			}

			_dbcontext.ShoppingListsProducts.Remove(record);

			await _dbcontext.SaveChangesAsync();
		}

		public async Task AddNewProductToFridgeAndShoppingList(string userId, ShoppingListProductViewModel product)
		{
			var category = (await _productService.GetAllCategories()).FirstOrDefault(x => x.Name == product.CategoryName);

			if (category == null)
			{
				throw new ArgumentNullException();
			}

			var prodId = await _productService.AddProduct(userId, new ProductFormViewModel
			{
				Name = product.Name,
				SelectedCategoryId = category.Id,
				Count = 0,
			});

			product.Id = prodId;

			await AddProductToShoppingList(userId, product);
		}

		public async Task StartShopping(string userId)
		{
			var shoppingList = await _dbcontext.ShoppingLists
				.Include(x => x.ShoppingListProducts)
				.FirstOrDefaultAsync(x => x.UserId == userId);
			if (shoppingList == null)
			{
				throw new ArgumentNullException();
			}

			if (shoppingList.ShoppingListProducts.Any())
			{
				shoppingList.IsStarted = true;
				shoppingList.StartedOn = DateTime.Now;
				await _dbcontext.SaveChangesAsync();
			}

		}

		public async Task CancelShopping(string userId)
		{
			var shoppingList = await _dbcontext.ShoppingLists
				.Include(x => x.ShoppingListProducts)
				.FirstOrDefaultAsync(x => x.UserId == userId);
			if (shoppingList == null)
			{
				throw new ArgumentNullException();
			}

			shoppingList.IsStarted = false;

			foreach (var item in shoppingList.ShoppingListProducts)
			{
				item.IsBought = false;
			}

			await _dbcontext.SaveChangesAsync();
		}

		public async Task<ShoppingListProductsByCategoryViewModel> GetProductsByCategory(string userId, int page, int prodOnPage = 20)
		{
			var products = _dbcontext.ShoppingListsProducts
				.AsNoTracking()
				.Where(x => x.ShoppingListId == userId);

			ShoppingListProductsByCategoryViewModel model = new();

			model.BoughtProducts = await products
				.Where(x => x.IsBought)
				.Select(x => new ShoppingListProductViewModel()
				{
					Id = x.ProductId,
					Name = x.Product.Name,
					Price = x.StorePrice,
					QuantityToBuy = x.QuantityToBuy,
					CategoryName = x.Product.Category.Name,
				}).ToListAsync();

			int totalProducts = await products.CountAsync();

			model.Progress = model.BoughtProducts.Count * 100 / totalProducts;

			model.TotalPages = (int)Math.Ceiling((totalProducts-model.BoughtProducts.Count) / (double)prodOnPage);

			if (page < 1)
			{
				page = 1;
			}
			else if (page > model.TotalPages && model.TotalPages != 0)
			{
				page = model.TotalPages;
			}

			model.PageNumber = page;

			var extractedProd = await products
				.Include(x => x.Product)
				.ThenInclude(x => x.Category)
				.OrderBy(x => x.Product.Category.Id)
				.Where(x => !x.IsBought)
				.Skip((page - 1) * prodOnPage)
				.Take(prodOnPage)
				.ToListAsync();


			foreach (var p in extractedProd)
			{
				if (!model.UnboughtProductsByCategory.ContainsKey(p.Product.Category.Name))
				{
					model.UnboughtProductsByCategory.Add(p.Product.Category.Name, new());
				}

				model.UnboughtProductsByCategory[p.Product.Category.Name]
					.Add(new ShoppingListProductViewModel()
					{
						Id = p.ProductId,
						Name = p.Product.Name,
						Price = p.StorePrice,
						QuantityToBuy = p.QuantityToBuy,
					});
			}

			return model;
		}

		public async Task MarkAsBought(string userId, int prodId)
		{
			var prod = await _dbcontext.ShoppingListsProducts
				.FirstOrDefaultAsync(x => x.ShoppingListId == userId && x.ProductId == prodId);

			if (prod == null)
			{
				throw new ArgumentNullException(nameof(prod));
			}

			prod.IsBought = true;
			await _dbcontext.SaveChangesAsync();
		}

		public async Task MarkAsUnbought(string userId, int productId)
		{
			var prod = await _dbcontext.ShoppingListsProducts
				.FirstOrDefaultAsync(x => x.ShoppingListId == userId && x.ProductId == productId);

			if (prod == null)
			{
				throw new ArgumentNullException(nameof(prod));
			}

			prod.IsBought = false;
			await _dbcontext.SaveChangesAsync();
		}

		public async Task SaveBoughtProducts(string userId)
		{
			var sl = await _dbcontext.ShoppingLists
				.Include(x => x.ShoppingListProducts)
				.FirstOrDefaultAsync(x => x.UserId == userId);

			if (sl == null)
			{
				throw new ArgumentNullException(nameof(sl));
			}

			var productsToUpdate = await _dbcontext.Products
				.Where(x => sl.ShoppingListProducts.Select(y => y.ProductId).Contains(x.Id))
				.ToListAsync();

			productsToUpdate.ForEach(x =>
			{
				x.Count += sl.ShoppingListProducts.First(y => y.ProductId == x.Id).QuantityToBuy;
				x.AddedOn = DateTime.Now;
			});

			sl.IsFinished = true;

			await _dbcontext.SaveChangesAsync();

		}

		public async Task<IEnumerable<ShoppingListViewModel>>
			GetTop20StartedShoppingListsExceptCurrentUser(string userId)
		{
			var shoppingLists = await _dbcontext.ShoppingLists
				.AsNoTracking()
				.Take(20)
				.Where(x => x.UserId != userId && x.IsStarted)
				.Select(sl=>new ShoppingListViewModel()
				{
					UserId=sl.UserId,
					StartedOn=sl.StartedOn,
					FirstName = sl.User.FirstName,
					LastName= sl.User.LastName,
					Products = sl.ShoppingListProducts.Select(x => new ShoppingListProductViewModel()
					{
						Id = x.ProductId,
						Name = x.Product.Name
					}),
					Progress = sl.ShoppingListProducts.Where(x => x.IsBought).Count() * 100 / sl.ShoppingListProducts.Count()
					
				}).ToListAsync();


            for (int i = 0; i < shoppingLists.Count; i++)
            {
				shoppingLists[i].ProfilePicture = await _ImageService.GetPFP(shoppingLists[i].UserId);
			}

			return shoppingLists;
		}
	}
}

