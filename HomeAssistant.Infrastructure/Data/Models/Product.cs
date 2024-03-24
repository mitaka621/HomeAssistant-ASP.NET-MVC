using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Infrastructure.Data.Models
{
    [Comment("Product")]
    public class Product
    {
        [Key]
        [Comment("Product identifier")]
        public int Id { get; set; }

        [Required]
        [MaxLength(Constants.NameMaxLenght)]
        [Comment("Product name")]
        public string Name { get; set; }=string.Empty;

        [Required]
        [Comment("Product category")]
        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; } = null!;

        [Required]
        [Comment("Product Count")]
        public int Count { get; set; }

        [Comment("Product weight in grams (optional)")]
        public int? Weight { get; set; }

        [Comment("Date and time for when the product was added")]
        public DateTime AddedOn { get; set; }

        [Comment("UserId who added the product (optional)")]
        public string? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public HomeAssistantUser? User { get; set; }

        public IEnumerable<ShoppingListProduct> ProductShoppingLists { get; set; } = new List<ShoppingListProduct>();

		public IEnumerable<RecipeProduct> ProductRecipes { get; set; } = new List<RecipeProduct>();
	}
}
