using HomeAssistant.Core.Enums;
using HomeAssistant.Core.Models;
using HomeAssistant.Core.Models.Fridge;
using HomeAssistant.Core.Models.Product;

namespace HomeAssistant.Core.Contracts
{
	public interface IProductService
	{
		Task<FridgeViewModel> GetProducts(bool available, int? categoryId, OrderBy orderBy, int page, int productsOnPage = 10);

		Task<IEnumerable<CategoryViewModel>> GetAllCategories();

		Task<int> IncreaseQuantityByOne(int productId);

		Task<int> DecreaseQuantityByOne(int productId);

		Task<IEnumerable<ProductViewModel>> SearchProducts(string keyphrase);

		Task<ProductFormViewModel> GetProduct(int id);

		Task<int> AddProduct(string userId, ProductFormViewModel productViewModel);

		Task DeleteProduct(int prodId);

		Task EditProduct(string userId, ProductFormViewModel productViewModel);

	}
}
