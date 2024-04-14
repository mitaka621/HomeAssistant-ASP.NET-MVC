using HomeAssistant.Core.Models.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Models.Fridge
{
    public class FridgeViewModel
    {
        public int PageCount { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<ProductViewModel> Products { get; set; } = new List<ProductViewModel>();

        public IEnumerable<NotificationViewModel> LatestFridgeActivity { get; set; }= new List<NotificationViewModel>();
    }
}
