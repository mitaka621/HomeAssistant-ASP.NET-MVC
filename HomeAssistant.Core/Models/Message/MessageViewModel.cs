namespace HomeAssistant.Core.Models.Message
{
	public class MessageViewModel
	{
		public string MessageContent { get; set; } = string.Empty;

        public DateTime CreatedOn { get; set; }

		public string UserId { get; set; } = string.Empty;
    }
}
