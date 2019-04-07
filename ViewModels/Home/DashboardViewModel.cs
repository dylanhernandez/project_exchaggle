using System;
using System.Collections.Generic;

namespace Exchaggle.ViewModels.Home
{
    public class DashboardViewModel
    {
        public string Username { get; set; }
        public IEnumerable<NotificationViewModel> Notifications { get; set; }
    }
}