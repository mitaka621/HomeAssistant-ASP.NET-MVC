﻿using HomeAssistant.Core.Constants;
using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Enums;
using HomeAssistant.Core.Models;
using HomeAssistant.Core.Models.Fridge;
using HomeAssistant.Core.Models.Product;
using HomeAssistant.Infrastructure.Data;
using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace HomeAssistant.Core.Services
{
    public class ProductService : IProductService
	{
		private readonly HomeAssistantDbContext _dbContext;

		public ProductService(HomeAssistantDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<FridgeViewModel> GetProducts(
			bool available,
			int? categoryId,
			OrderBy orderBy,
			int page,
			int productsOnPage=20)
		{
			var prodToReturn = _dbContext.Products
				.AsNoTracking()
				.Where(x => (categoryId ?? x.CategoryId) == x.CategoryId);

			if (available)
			{
				prodToReturn = prodToReturn.Where(x => x.Count > 0);
			}
			else
			{
				prodToReturn = prodToReturn.Where(x => x.Count == 0);
			}

			switch (orderBy)
			{
				case OrderBy.Recent:
					prodToReturn = prodToReturn.OrderByDescending(x => x.AddedOn)
						.ThenBy(x => x.Id);
					break;
				case OrderBy.Oldest:
					prodToReturn = prodToReturn.OrderBy(x => x.AddedOn)
						.ThenBy(x => x.Id);
					break;
				case OrderBy.Name:
					prodToReturn = prodToReturn.OrderBy(x => x.Name)
						.ThenBy(x => x.Id);
					break;
			}
			FridgeViewModel finalModel = new();

			finalModel.PageCount = (int)Math.Ceiling((await prodToReturn.CountAsync()) / (double)productsOnPage);

			if (page < 1)
			{
				page = 1;
			}
			else if (page > finalModel.PageCount && finalModel.PageCount != 0)
			{
				page = finalModel.PageCount;
			}

			finalModel.CurrentPage = page;

			prodToReturn = prodToReturn
				.Skip((page - 1) * productsOnPage)
				.Take(productsOnPage);

			finalModel.Products = await prodToReturn
				.Select(x => new ProductViewModel()
				{
					Id = x.Id,
					Name = x.Name,
					AddedOn = x.AddedOn,
					ProductCategory = new CategoryViewModel()
					{
						Id = x.CategoryId,
						Name = x.Category.Name,
					},
					Count = x.Count,
					Weight = x.Weight,
					User = x.User != null ? new UserDetailsViewModel()
					{
						Id = x.User.Id,
						FirstName = x.User.FirstName,
						LastName = x.User.LastName,
						Username = x.User.UserName,
						Email = x.User.Email,
						CreatedOn = x.User.CreatedOn.ToString(DataValidationConstants.DateTimeFormat),
					} : null
				})
			.ToListAsync();

			return finalModel;
		}


		public async Task<IEnumerable<CategoryViewModel>> GetAllCategories()
		{
			return await _dbContext.Categories
				.AsNoTracking()
				.Select(x => new CategoryViewModel()
				{
					Id = x.Id,
					Name = x.Name,
				}).ToListAsync();
		}

		public async Task<int> IncreaseQuantityByOne(int productId)
		{
			var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == productId);

			if (product == null)
			{
				throw new ArgumentNullException();
			}

			if (product.Count < 0)
			{
				throw new InvalidOperationException();
			}

			product.Count++;
			product.AddedOn = DateTime.Now;

			await _dbContext.SaveChangesAsync();

			return product.Count;
		}

		public async Task<int> DecreaseQuantityByOne(int productId)
		{
			var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == productId);

			if (product == null)
			{
				throw new ArgumentNullException();
			}

			if (product.Count <= 0)
			{
				throw new InvalidOperationException();
			}

			product.Count--;
			await _dbContext.SaveChangesAsync();

			return product.Count;
		}

		public async Task<IEnumerable<ProductViewModel>> SearchProducts(string keyphrase)
		{
			var products = await _dbContext.Products
				.AsNoTracking()
				.Where(x => x.Name.Contains(keyphrase))
				.Take(10)
				.Select(x => new ProductViewModel()
				{
					Id = x.Id,
					Name = x.Name,
					AddedOn = x.AddedOn,
					ProductCategory = new CategoryViewModel()
					{
						Id = x.CategoryId,
						Name = x.Category.Name,
					},
					Count = x.Count,
					Weight = x.Weight,
					User = x.User != null ? new UserDetailsViewModel()
					{
						Id = x.User.Id,
						FirstName = x.User.FirstName,
						LastName = x.User.LastName,
						Username = x.User.UserName,
						Email = x.User.Email,
						CreatedOn = x.User.CreatedOn.ToString(DataValidationConstants.DateTimeFormat),
					} : null
				}).ToListAsync();

			return products;
		}

		public async Task<ProductFormViewModel> GetProduct(int id)
		{
			var product = await _dbContext.Products
				.AsNoTracking()
				.Include(x => x.Category)
				.FirstOrDefaultAsync(x => x.Id == id);

			if (product == null)
			{
				throw new ArgumentNullException();
			}
			return new ProductFormViewModel()
			{
				Id = product.Id,
				Name = product.Name,
				AddedOn = product.AddedOn,
				ProductCategory = new CategoryViewModel()
				{
					Id = product.CategoryId,
					Name = product.Category.Name,
				},
				AllCategories = await GetAllCategories(),
				Count = product.Count,
				Weight = product.Weight,

			};
		}

		public async Task<int> AddProduct(string userId, ProductFormViewModel product)
		{
			var p = new Product()
			{
				Name = product.Name,
				AddedOn = DateTime.Now,
				CategoryId = product.SelectedCategoryId,
				Count = product.Count,
				Weight = product.Weight,
				UserId = userId

			};
			_dbContext.Products.Add(p);

			await _dbContext.SaveChangesAsync();

			return p.Id;
		}

		public async Task DeleteProduct(int prodId)
		{
			var product = await _dbContext.Products
				.FirstOrDefaultAsync(x => x.Id == prodId);

			if (product == null)
			{
				throw new ArgumentNullException();
			}

			_dbContext.Products.Remove(product);

			await _dbContext.SaveChangesAsync();

		}

		public async Task EditProduct(string userId, ProductFormViewModel productViewModel)
		{
			var product = await _dbContext.Products
				.FirstOrDefaultAsync(x => x.Id == productViewModel.Id);

			if (product == null)
			{
				throw new ArgumentNullException();
			}

			product.Name = productViewModel.Name;
			product.Weight = productViewModel.Weight;
			product.UserId = userId;
			product.Count = productViewModel.Count;
			product.AddedOn = DateTime.Now;
			product.CategoryId = productViewModel.SelectedCategoryId;

			await _dbContext.SaveChangesAsync();
		}

		public async Task UpdateMultipleProductsQuantities(Dictionary<int, int> products)
		{
			var dbProducts=await _dbContext.Products.Where(x => products.Keys.Contains(x.Id)).ToListAsync();

            foreach (var item in dbProducts)
            {
				item.Count = products[item.Id];
			}

			await _dbContext.SaveChangesAsync();
        }
	}
}
