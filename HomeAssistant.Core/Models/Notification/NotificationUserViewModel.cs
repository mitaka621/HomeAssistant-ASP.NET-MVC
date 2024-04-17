namespace HomeAssistant.Core.Models.Notification
{
	public class NotificationUserViewModel
	{
		public string? Id { get; set; } = null!;
        public byte[] Photo { get; set; } = null!;

		public string? FirstName { get; set; } = string.Empty;


    }
}
