using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Infrastructure.Data.Models
{
    public class HomeAssistantUser : IdentityUser
    {
        [Required]
        [MaxLength(Constants.NameMaxLenght)]
        [PersonalData]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [PersonalData]
        [MaxLength(Constants.NameMaxLenght)]
        public string LastName { get; set; } = string.Empty;

        public DateTime CreatedOn { get; set; }

		public DateTime? DeletedOn { get; set; }

		[Required]
        public bool IsDeleted { get; set; } = false;

		[Column(TypeName = "float")]
		[DefaultValue(42.698334)]
        public double Latitude { get; set; }

        [Column(TypeName = "float")]
		[DefaultValue(23.319941)]
		public double Longitude { get; set; }

	}
}
