using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Infrastructure.Data.Models
{
	public class ShoppingListProduct
	{
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;

        public string ShoppingListId { get; set; }=string.Empty;
        [ForeignKey(nameof(ShoppingListId))]
        public ShoppingList ShoppingList { get; set; } = null!;

        public bool IsBought { get; set; }

        public double? StorePrice { get; set; }

        public int QuantityToBuy { get; set; }
    }
}
