using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeAssistant.Infrastructure.Data.Models
{
	public class Message
	{
        [Comment("Message Identifier")]
        [Key]
        public int MessageId { get; set; }

		[Comment("Chat room Identifier")]
		public int ChatRoomId { get; set; }
        [ForeignKey(nameof(ChatRoomId))]
        public ChatRoom ChatRoom { get; set; } = null!;

		[Comment("User id who wrote the message")]
		public string UserId { get; set; } = string.Empty;
        [ForeignKey(nameof(UserId))]
        public HomeAssistantUser User { get; set; } = null!;

		[Comment("Message text content")]
		public string MessageContent { get; set; } = string.Empty;

		[Comment("Message created on")]
		public DateTime CreatedOn { get; set; }

    }
}
