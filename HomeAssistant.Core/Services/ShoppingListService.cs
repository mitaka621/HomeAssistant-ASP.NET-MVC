using Amazon.Runtime.Internal;
using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Enums;
using HomeAssistant.Core.Models;
using HomeAssistant.Core.Models.Product;
using HomeAssistant.Core.Models.ShoppingList;
using HomeAssistant.Infrastructure.Data;
using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Services
{
	public class ShoppingListService : IShoppingListService
	{
		private readonly HomeAssistantDbContext _dbcontext;
		private readonly IProductService _productService;
		public ShoppingListService(HomeAssistantDbContext dbContext, IProductService productService)
		{
			_dbcontext = dbContext;
			_productService = productService;
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

		public async Task<bool> AnyUnboughtProducts(string userId)
		{
			return await _dbcontext
				.ShoppingListsProducts
				.AnyAsync(x => x.ShoppingListId == userId && !x.IsBought);
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
					Id=x.ProductId,
					Name = x.Product.Name,
					CategoryName = x.Product.Category.Name,
					Price = x.StorePrice,
					QuantityToBuy = x.QuantityToBuy,
				})
				.ToListAsync();

			return new ShoppingListViewModel()
			{
				OutOfStockProducts = (await _productService.GetProducts(false, null, OrderBy.Oldest, 0))
					.Products.Where(x=>!products.Any(y=>y.Id==x.Id)),
				Products = products,
				AllCategories = await _productService.GetAllCategories()
			};
		}

		public async Task DeleteShoppingList(string userId)
		{
			var sl = await _dbcontext.ShoppingLists
				.AsNoTracking()
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
				StorePrice=product.Price,
				QuantityToBuy=product.QuantityToBuy,
			});

			await _dbcontext.SaveChangesAsync();

		}

		public async Task DeleteProductFromShoppingList(string userId, int productId)
		{
			var record=await _dbcontext.ShoppingListsProducts
				.FirstOrDefaultAsync(x => x.ShoppingListId == userId && x.ProductId == productId);

            if (record==null)
            {
				throw new ArgumentNullException();
            }

			_dbcontext.ShoppingListsProducts.Remove(record);

			await _dbcontext.SaveChangesAsync();
        }

		public async Task AddNewProductToFridgeAndShoppingList(string userId, ShoppingListProductViewModel product)
		{
			var category = (await _productService.GetAllCategories()).FirstOrDefault(x => x.Name == product.CategoryName);

            if (category==null)
            {
				throw new ArgumentNullException();
            }

            var prodId=await _productService.AddProduct(userId, new ProductFormViewModel
			{
				Name = product.Name,
				SelectedCategoryId = category.Id,
				Count = 0,
				Weight = 0,
			});

			product.Id = prodId;

			await AddProductToShoppingList(userId, product);
		}

		public async Task StartShopping(string userId)
		{
			var shoppingList = await _dbcontext.ShoppingLists.FirstOrDefaultAsync(x => x.UserId == userId);
			if (shoppingList==null)
			{
				throw new ArgumentNullException();
			}

			shoppingList.IsStarted = true;

			await _dbcontext.SaveChangesAsync();
		}
	}
}
