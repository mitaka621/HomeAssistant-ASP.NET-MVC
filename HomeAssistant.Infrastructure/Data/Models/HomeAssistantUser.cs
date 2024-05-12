using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HomeAssistant.Infrastructure.Data.Models
{
	public class HomeAssistantUser : IdentityUser
    {
        [Comment("First name")]
        [Required]
        [MaxLength(Constants.NameMaxLenght)]
        [PersonalData]
        public string FirstName { get; set; } = string.Empty;


		[Comment("Last name")]
		[Required]
        [PersonalData]
        [MaxLength(Constants.NameMaxLenght)]
        public string LastName { get; set; } = string.Empty;

		[Comment("User creation date")]
		public DateTime CreatedOn { get; set; }

		[Comment("User delition date if the user is deleted")]
		public DateTime? DeletedOn { get; set; }

        [Comment("Is the user deleted")]
		[Required]
        public bool IsDeleted { get; set; } = false;

		[Comment("User location (Latitude) - default is in Sofia/Bulgaria")]
		[Column(TypeName = "float")]
		[DefaultValue(42.698334)]
        public double Latitude { get; set; }

		[Comment("User location (Longitude) - default is in Sofia/Bulgaria")]
		[Column(TypeName = "float")]
		[DefaultValue(23.319941)]
		public double Longitude { get; set; }

        public string ClientIpAddress { get; set; } = string.Empty;

        public DateTime? ExpiresOn { get; set; }

        public ShoppingList ShoppingList { get; set; } = null!;

        public IEnumerable<Notification> InvokedNotifications { get; set; } =new List<Notification>();

        public IEnumerable<UserStep> UserRecipeSteps { get; set; } = new List<UserStep>();
    }
}
