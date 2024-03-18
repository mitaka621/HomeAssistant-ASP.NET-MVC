using HomeAssistant.Core.Models.Product;
using HomeAssistant.Core.Models.ShoppingList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Contracts
{
    public interface IShoppingListService
	{
		public Task<bool> IsStarted(string userId);

		public Task<bool> AnyUnboughtProducts(string userId);

		public Task<ShoppingListViewModel> GetShoppingList(string userId);

		public Task DeleteShoppingList(string userId);

		public Task AddProductToShoppingList(string userId, ShoppingListProductViewModel product);

		public Task DeleteProductFromShoppingList(string userId, int productId);

		public Task AddNewProductToFridgeAndShoppingList(string userId, ShoppingListProductViewModel product);

		public Task StartShopping(string userId);
	}
}
