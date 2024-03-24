using System.ComponentModel.DataAnnotations.Schema;

namespace HomeAssistant.Infrastructure.Data.Models
{
	public class RecipeProduct
	{
        public int RecipeId { get; set; }
        [ForeignKey(nameof(RecipeId))]
        public Recipe Recipe { get; set; } = null!;

        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;

		public int? StepNumber { get; set; }
		[ForeignKey("RecipeId, StepNumber")]
		public Step? Step { get; set; }
	}
}