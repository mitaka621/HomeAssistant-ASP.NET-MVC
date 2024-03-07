using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Models
{
    internal class UserDetailsViewModel
    {
        public string Email { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string Roles { get; set; } = string.Empty;

        public string CreatedOn { get; set; } = string.Empty;
    }
}
