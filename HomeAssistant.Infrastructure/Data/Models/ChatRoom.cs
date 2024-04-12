using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Infrastructure.Data.Models
{
	public class ChatRoom
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string User1Id { get; set; } = null!;
		[ForeignKey(nameof(User1Id))]
		public HomeAssistantUser User1 { get; set; } = null!;

		[Required]
		public string User2Id { get; set; } = null!;
		[ForeignKey(nameof(User2Id))]
		public HomeAssistantUser User2 { get; set; } = null!;

		public IEnumerable<Message> Messages { get; set; } = new List<Message>();
	}
}
