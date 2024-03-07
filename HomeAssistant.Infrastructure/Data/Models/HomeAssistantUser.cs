using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

    }
}
