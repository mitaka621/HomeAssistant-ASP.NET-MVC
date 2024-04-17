using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeAssistant.Infrastructure.Data.Models
{
	public class RecipeProduct
	{
        [Comment("Id of the recipe")]
        public int RecipeId { get; set; }
        [ForeignKey(nameof(RecipeId))]
        public Recipe Recipe { get; set; } = null!;

		[Comment("Id of the product associated with the recipe")]
		public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;

        [Comment("Quantity of the selected product required for the recipe")]
        public int Quantity { get; set; }

        public ICollection<RecipeProductStep> Steps { get; set; }=new List<RecipeProductStep>();
	}
}