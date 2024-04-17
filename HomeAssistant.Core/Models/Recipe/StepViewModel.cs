using HomeAssistant.Infrastructure.Data.Enums;

namespace HomeAssistant.Core.Models.Recipe
{
	public class StepViewModel
	{
        public int StepNumber { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public StepType Type { get; set; }

        public int? Duration { get; set; }
    }
}
