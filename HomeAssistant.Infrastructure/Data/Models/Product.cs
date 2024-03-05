using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Comment("Prouct category")]
        public Category Category { get; set; } = null!;

        [Required]
        [Comment("Product Count")]
        public int Count { get; set; }

        [Comment("Product weight in grams (optional)")]
        public int? Weight { get; set; }

        [Comment("Date and time for when the product was added")]
        public DateTime AddedOn { get; set; }

        [Comment("User which added the product (optional)")]
        public IdentityUser? User { get; set; }
    }
}
