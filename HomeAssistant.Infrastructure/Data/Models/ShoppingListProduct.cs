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

        public int ShoppingListId { get; set; }
        [ForeignKey(nameof(ShoppingListId))]
        public ShoppingList ShoppingList { get; set; } = null!;
    }
}
