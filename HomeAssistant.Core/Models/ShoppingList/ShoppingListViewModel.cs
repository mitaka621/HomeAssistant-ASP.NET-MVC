using HomeAssistant.Core.Models.Product;

namespace HomeAssistant.Core.Models.ShoppingList
{
    public class ShoppingListViewModel
    {
        public string UserId { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public DateTime? StartedOn { get; set; }

        public int? Progress { get; set; }

        public IEnumerable<ShoppingListProductViewModel> Products { get; set; } = new List<ShoppingListProductViewModel>();

        public IEnumerable<ProductViewModel> OutOfStockProducts { get; set; } = new List<ProductViewModel>();

        public IEnumerable<CategoryViewModel> AllCategories { get; set; } = new List<CategoryViewModel>();

        public bool IsStarted { get; set; }

        public bool IsFinished { get; set; }
    }
}
