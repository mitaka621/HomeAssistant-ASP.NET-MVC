namespace HomeAssistant.Core.Models
{
	public class UserDetailsViewModel
    {
        public string Id { get; set; }=string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string Roles { get; set; } = string.Empty;

        public string CreatedOn { get; set; } = string.Empty;

		public string? DeletedOn { get; set; } = string.Empty;

        public byte[] Photo { get; set; } = new byte[0];

    }
}
