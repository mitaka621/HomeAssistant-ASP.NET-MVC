using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Models.WakeOnLanModels
{
	public class PcModel
	{
        public string MAC { get; set; }=string.Empty;

        public string PcName { get; set; }=string.Empty;

        public string PcIp { get; set; }=string.Empty;

        public bool IsHostPc { get; set; }

        public bool IsNAS { get; set; }
    }
}
