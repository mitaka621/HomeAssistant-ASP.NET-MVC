using HomeAssistant.Core.Constants;
using System.ComponentModel.DataAnnotations;

namespace HomeAssistant.Core.Models.ShoppingList
{
	public class ShoppingListProductViewModel
	{
        public int Id { get; set; }

        [Required(ErrorMessage =ErrorMessages.RequiredField)]
        public string Name { get; set; } = string.Empty;

        [Range(0,100000,ErrorMessage =ErrorMessages.ValueShouldBeInBetween)]
        public double? Price { get; set; }

		[Range(1, int.MaxValue, ErrorMessage = ErrorMessages.ShouldBeGreaterThanZero)]
		public int QuantityToBuy { get; set; }

        public string CategoryName { get; set; } = string.Empty;
    }
}
