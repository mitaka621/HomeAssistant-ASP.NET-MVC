namespace HomeAssistant.Core.Models.Recipe
{
	public class RecipeProductViewModel
	{
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
		public int AvailableQuantity { get; set; }
		public bool IsAvailable { get; set; }
    }
}
