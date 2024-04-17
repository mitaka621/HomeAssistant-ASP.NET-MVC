using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeAssistant.Infrastructure.Data.Models
{
	public class ShoppingList
	{
		[Comment("User identifier")]
		[Key]
		public string UserId { get; set; } = string.Empty;
		[ForeignKey(nameof(UserId))]
		public HomeAssistantUser User { get; set; } = null!;

		[Comment("Is the shopping list started")]
		public bool IsStarted { get; set; }

		[Comment("Is shopping list finished")]
		public bool IsFinished { get; set; }

		[Comment("Shopping list started on")]
        public DateTime? StartedOn { get; set; }

        public IEnumerable<ShoppingListProduct> ShoppingListProducts { get; set; } = new List<ShoppingListProduct>();
    }
}
