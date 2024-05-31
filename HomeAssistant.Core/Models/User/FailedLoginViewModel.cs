using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Models.User
{
    public class FailedLoginViewModel
    {
        public string Ip { get; set; }=string.Empty;

        public int AttemptsCount { get; set; }

        public DateTime LastAttemptOn { get; set; }
    }
}
