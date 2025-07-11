﻿using HomeAssistant.Infrastructure.Data.Enums;

namespace HomeAssistant.Core.Models.Recipe
{
    public class StepDetailsViewModel
    {
        public int RecipeId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int? Duration { get; set; }

        public DateTime? InitiatedOn { get; set; }

        public int StepNumber { get; set; }

        public StepType Type { get; set; }

        public IEnumerable<string> Products { get; set; } = new List<string>();

        public int TotalStepsCount { get; set; }
    }
}
