using HomeAssistant.Core.Models.Product;

namespace HomeAssistant.Core.Models
{
	public class ProductViewModel
	{
		public int Id { get; set; }

		public string Name { get; set; } = string.Empty;

		public int Count { get; set; }

		public int? Weight { get; set; }

		public DateTime AddedOn { get; set; }

		public UserDetailsViewModel? User { get; set; }

		public CategoryViewModel ProductCategory { get; set; } = null!;

	}
}
