using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Infrastructure.Data.Models
{
	public class BlacklistedIp
	{
		[Key]
		public string Ip { get; set; } = string.Empty;

        public DateTime LastTry { get; set; }

        public int Count { get; set; }
    }
}
