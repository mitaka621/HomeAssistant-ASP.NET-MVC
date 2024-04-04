namespace HomeAssistant.Core.Models.Recipe
{
	public class RecipeDetaislViewModel
	{
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; }=null!;

        public byte[]? Photo { get; set; }

        public IEnumerable<RecipeProductViewModel> Products { get; set; }=new List<RecipeProductViewModel>();

        public IEnumerable<StepViewModel> Steps { get; set; } = new List<StepViewModel>();

        public bool AnySteps { get; set; }

        public int PercentageCompleted { get; set; } = -1;
    }
}
