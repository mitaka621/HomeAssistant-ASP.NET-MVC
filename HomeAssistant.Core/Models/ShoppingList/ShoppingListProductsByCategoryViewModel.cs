using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Models.ShoppingList
{
	public class ShoppingListProductsByCategoryViewModel
	{
		public Dictionary<string, List<ShoppingListProductViewModel>> UnboughtProductsByCategory { get; set; } = new();

        public List<ShoppingListProductViewModel> BoughtProducts { get; set; } = new();

        public int PageNumber { get; set; }

        public int Progress { get; set; }

        public int TotalPages { get; set; }
    }
}
