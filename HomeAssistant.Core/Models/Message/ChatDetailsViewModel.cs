namespace HomeAssistant.Core.Models.Message
{
    public class ChatDetailsViewModel
    {
        public int ChatRoomId { get; set; }

        public string currentUserId { get; set; } = string.Empty;

        public string UserId2 { get; set; } = string.Empty;

        public string Username2 { get; set; } = string.Empty;

        public IEnumerable<MessageViewModel> Messages { get; set; } = new List<MessageViewModel>();
    }
}
