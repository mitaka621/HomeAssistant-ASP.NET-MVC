using HomeAssistant.Core.Constants;
using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Models;
using HomeAssistant.Infrastructure.Data;
using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Services
{
	public class ProductService : IProductService
	{
		private readonly HomeAssistantDbContext _dbContext;

		public ProductService(HomeAssistantDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<IEnumerable<ProductViewModel>> GetProducts(bool available, int? categoryId)
		{
			if (available)
			{
				return await _dbContext.Products
					.Where(x => x.Count > 0 && ((categoryId ?? x.CategoryId) == x.CategoryId))
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
					}).AsNoTracking()
					.ToListAsync();
			}

			return await _dbContext.Products
					.Where(x => x.Count == 0 && ((categoryId ?? x.CategoryId) == x.CategoryId))
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
					}).AsNoTracking()
					.ToListAsync();
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

		public async Task IncreaseQuantityByOne(int productId)
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
		}

		public async Task DecreaseQuantityByOne(int productId)
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
		}

		public async Task<IEnumerable<ProductViewModel>> SearchProducts(string keyphrase)
		{
			var products = await _dbContext.Products
				.Where(x => x.Name.Contains(keyphrase))
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
				}).AsNoTracking()
					.ToListAsync();

			return products;
		}

		public async Task<ProductFormViewModel> GetProduct(int id)
		{
			var product = await _dbContext.Products.Include(x=>x.Category)
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
	}
}
