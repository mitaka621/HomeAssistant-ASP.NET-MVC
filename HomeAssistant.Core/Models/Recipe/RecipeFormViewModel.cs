using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static HomeAssistant.Core.Constants.ErrorMessages;
using static HomeAssistant.Core.Constants.DataValidationConstants;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace HomeAssistant.Core.Models.Recipe
{
	public class RecipeFormViewModel
	{
        public IFormFile? RecipeImage { get; set; }

        public int Id { get; set; }

		[Required(ErrorMessage = RequiredField)]
		[StringLength(NameMaxLenght,
			MinimumLength = NameMinLenght,
			ErrorMessage = InvalidStringLength)]
		public string Name { get; set; } = string.Empty;

		[Required(ErrorMessage = RequiredField)]
		[StringLength(DescriptionMaxLength,
			MinimumLength = DescriptionMinLength,
			ErrorMessage = InvalidStringLength)]
		public string Description { get; set; } = string.Empty;

        public IEnumerable<int> ProductsIds { get; set; } = new List<int>();

		public Dictionary<int, string> Products { get; set; } = new();
    }
}
