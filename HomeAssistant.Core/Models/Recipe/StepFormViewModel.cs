﻿using System.ComponentModel.DataAnnotations;
using static HomeAssistant.Core.Constants.ErrorMessages;
using static HomeAssistant.Core.Constants.DataValidationConstants;
using Microsoft.EntityFrameworkCore;
using HomeAssistant.Infrastructure.Data.Enums;

namespace HomeAssistant.Core.Models.Recipe
{
	public class StepFormViewModel
	{
        public int RecipeId { get; set; }

		public int StepNumber { get; set; }

        [Required(ErrorMessage = RequiredField)]
		[StringLength(NameMaxLenght,
		   MinimumLength = NameMinLenght,
		   ErrorMessage = InvalidStringLength)]
		[Comment("Product name")]
		public string Name { get; set; } = string.Empty;

		[Required(ErrorMessage = RequiredField)]
		[StringLength(DescriptionMaxLength,
		   MinimumLength = DescriptionMinLength,
		   ErrorMessage = InvalidStringLength)]
		[Comment("Product name")]
		public string Description { get; set; } = string.Empty;

		[Range(0,int.MaxValue,ErrorMessage =ShouldBeGreaterThanZero)]
		[Display(Name="Duration in minutes:")]
        public int? Duration { get; set; }

		public int[] SelectedProductIds { get; set; }= new int[0];

        public StepDetailsViewModel? PreviousStep { get; set; }

		public IEnumerable<RecipeProductViewModel> Products { get; set; } = new List<RecipeProductViewModel>();

		public StepType StepType { get; set; }	
    }
}
