using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeAssistant.Infrastructure.Data.Models
{
	public class ShoppingListProduct
	{
        [Comment("Product Identifier")]
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;

		[Comment("Shopping List Identifier")]
		public string ShoppingListId { get; set; }=string.Empty;
        [ForeignKey(nameof(ShoppingListId))]
        public ShoppingList ShoppingList { get; set; } = null!;

		[Comment("Is the product currently in the shopping list bought?")]
		public bool IsBought { get; set; }

		[Comment("Optional store price which is useful only for the shopping list")]
		public double? StorePrice { get; set; }

		[Comment("Quantity to buy which will be transfered to the fridge ones the shopping is done")]
        public int QuantityToBuy { get; set; }
    }
}
