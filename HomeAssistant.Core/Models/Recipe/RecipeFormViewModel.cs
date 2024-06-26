﻿using System.ComponentModel.DataAnnotations;
using static HomeAssistant.Core.Constants.ErrorMessages;
using static HomeAssistant.Core.Constants.DataValidationConstants;
using Microsoft.AspNetCore.Http;

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

        public IEnumerable<RecipeProductViewModel> SelectedProducts { get; set; } = new List<RecipeProductViewModel>();

		public IEnumerable<RecipeProductViewModel> RecipeProducts { get; set; } = new List<RecipeProductViewModel>();
	}
}
