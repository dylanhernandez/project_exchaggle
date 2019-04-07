using Exchaggle.ViewModels.Common;

namespace Exchaggle.ViewModels.Home
{
    public class NotificationViewModel
    {
        public int NotificationId { get; set; }
        public string DateTime { get; set; }
        public string Description { get; set; }
        public RedirectViewModel Link { get; set; }
    }
}