using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HomeAssistant.Infrastructure.Data.Models
{
	[Comment("Product category")]
    public class Category
    { 
        [Key]
        [Comment("Category Identifier")]
        public int Id { get; set; }

        [Comment("Category name")]
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
