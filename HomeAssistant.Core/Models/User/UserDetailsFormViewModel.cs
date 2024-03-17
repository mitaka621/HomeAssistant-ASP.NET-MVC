using static HomeAssistant.Core.Constants.ErrorMessages;
using static HomeAssistant.Core.Constants.DataValidationConstants;
using System.ComponentModel.DataAnnotations;
using Amazon.Util.Internal;
using Microsoft.AspNetCore.Http;
using HomeAssistant.Core.Models.User;

namespace HomeAssistant.Core.Models
{
    public class UserDetailsFormViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage =RequiredField)]
        [StringLength(NameMaxLenght,
            MinimumLength = NameMinLenght,
            ErrorMessage =InvalidStringLength)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredField)]
        [StringLength(NameMaxLenght,
            MinimumLength = NameMinLenght,
            ErrorMessage = InvalidStringLength)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredField)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredField)]
		[StringLength(NameMaxLenght,
			MinimumLength = NameMinLenght,
			ErrorMessage = InvalidStringLength)]
		public string Username { get; set; } = string.Empty;
        
        public string SelectedRoleId { get; set; } = string.Empty;

        public IEnumerable<string>? UserRoles { get; set; }

        public IEnumerable<RoleViewModel>? AllRoles { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
