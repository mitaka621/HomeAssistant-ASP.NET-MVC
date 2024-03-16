using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Infrastructure.Data.Models
{
	public class ShoppingList
	{
		[Key]
        public int Id { get; set; }

        public bool IsStarted { get; set; }

        public bool IsFinished { get; set; }

        [Required]
        public string UserId { get; set; }=string.Empty;
        [ForeignKey(nameof(UserId))]
        public HomeAssistantUser User { get; set; } = null!;

        public IEnumerable<ShoppingListProduct> ShoppingListProducts { get; set; } = new List<ShoppingListProduct>();
    }
}
