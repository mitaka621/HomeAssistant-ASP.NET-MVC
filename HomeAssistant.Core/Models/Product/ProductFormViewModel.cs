using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static HomeAssistant.Core.Constants.ErrorMessages;
using static HomeAssistant.Core.Constants.DataValidationConstants;
using System.ComponentModel;

namespace HomeAssistant.Core.Models.Product
{
    public class ProductFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = RequiredField)]
        [StringLength(NameMaxLenght,
            MinimumLength = NameMinLenght,
            ErrorMessage = InvalidStringLength)]
        [Comment("Product name")]
        public string Name { get; set; } = string.Empty;

        public CategoryViewModel? ProductCategory { get; set; }

        public int SelectedCategoryId { get; set; }

        public IEnumerable<CategoryViewModel> AllCategories { get; set; } = new List<CategoryViewModel>();

        [Required(ErrorMessage = RequiredField)]
        [Range(QuantityMin, QuantityMax, ErrorMessage = ValueShouldBeInBetween)]
        public int Count { get; set; }

        [DisplayName("Weight (grams)")]
        [Range(QuantityMin, int.MaxValue, ErrorMessage = ShouldBeGreaterThanZero)]
        public int? Weight { get; set; }

        public DateTime AddedOn { get; set; }
    }
}
