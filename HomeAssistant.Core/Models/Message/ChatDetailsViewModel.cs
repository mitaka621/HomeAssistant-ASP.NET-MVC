namespace HomeAssistant.Core.Models.Message
{
	public class ChatDetailsViewModel
	{
        public int ChatRoomId { get; set; }

        public string currentUserId { get; set; } = string.Empty;

		public string UserId2 { get; set; }= string.Empty;
		public string Username2 { get; set; } = string.Empty;

		public byte[] currentUserPhoto { get; set; } = new byte[0];

		public byte[] UserPhoto2 { get; set; } = new byte[0];

		public IEnumerable<MessageViewModel> Messages { get; set; } = new List<MessageViewModel>();
    }
}
