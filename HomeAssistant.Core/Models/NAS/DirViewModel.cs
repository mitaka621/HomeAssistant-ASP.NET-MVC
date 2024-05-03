﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Models.NAS
{
	public class DirViewModel
	{
		[JsonPropertyName("displayName")]
		public string DisplayName { get; set; } = string.Empty;

		[JsonPropertyName("path")]
		public string Path { get; set; } = string.Empty;

        [JsonPropertyName("isFile")]
        public int IsFile { get; set; }
    }
}