using HomeAssistant.Core.Enums;
using HomeAssistant.Core.Models;
using HomeAssistant.Core.Models.Fridge;
using HomeAssistant.Core.Models.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Contracts
{
    public interface IProductService
	{
		Task<FridgeViewModel> GetProducts(bool available, int? categoryId,OrderBy orderBy,int page);

		Task<IEnumerable<CategoryViewModel>> GetAllCategories();

		Task IncreaseQuantityByOne(int productId);

		Task DecreaseQuantityByOne(int productId);

		Task<IEnumerable<ProductViewModel>> SearchProducts(string keyphrase);

		Task<ProductFormViewModel> GetProduct(int id);

		Task AddProduct(string userId, ProductFormViewModel productViewModel);

		Task DeleteProduct(int prodId);

		Task EditProduct(string userId, ProductFormViewModel productViewModel);
	}
}
