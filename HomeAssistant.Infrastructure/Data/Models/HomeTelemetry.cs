using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Infrastructure.Data.Models
{
	public class HomeTelemetry
	{
        [Key]
        public int Id { get; set; }

        public DateTime DateTime { get; set; }

        public int Humidity { get; set; }

		public int Tempreture { get; set; }

		public int CPM { get; set; }

		public int Radiation { get; set; }
	}
}
