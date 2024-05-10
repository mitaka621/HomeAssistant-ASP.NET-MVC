using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Models.NAS
{
	public class PhotoPrevNextPaths
	{
		[JsonPropertyName("prevImg")]
		public string PreviousPath { get; set; } = string.Empty;

		[JsonPropertyName("nextImg")]
		public string NextPath { get; set; } = string.Empty;

    }
}
