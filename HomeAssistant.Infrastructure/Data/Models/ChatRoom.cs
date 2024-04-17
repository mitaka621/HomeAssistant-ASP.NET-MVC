using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace HomeAssistant.Infrastructure.Data.Models
{
	public class ChatRoom
	{
		[Comment("Chat romm identifier")]
		[Key]
		public int Id { get; set; }

		[Comment("User 1 Identifier")]
		[Required]
		public string User1Id { get; set; } = null!;
		[ForeignKey(nameof(User1Id))]
		public HomeAssistantUser User1 { get; set; } = null!;

		[Comment("User 2 Identifier")]
		[Required]
		public string User2Id { get; set; } = null!;
		[ForeignKey(nameof(User2Id))]
		public HomeAssistantUser User2 { get; set; } = null!;

		public IEnumerable<Message> Messages { get; set; } = new List<Message>();
	}
}
