using HomeAssistant.Core.Models.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Models.ShoppingList
{
    public class ShoppingListViewModel
    {
        public IEnumerable<ShoppingListProductViewModel> Products { get; set; } = new List<ShoppingListProductViewModel>();

        public IEnumerable<ProductViewModel> OutOfStockProducts { get; set; } = new List<ProductViewModel>();

        public IEnumerable<CategoryViewModel> AllCategories { get; set; } = new List<CategoryViewModel>();

		public bool IsStarted { get; set; }

        public bool IsFinished { get; set; }
    }
}
