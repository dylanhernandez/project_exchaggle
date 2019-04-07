using Exchaggle.Enumerations;
using Exchaggle.ViewModels.Common;

namespace Exchaggle.Interfaces
{
    interface INotifiable
    {
        void SaveNotification(dynamic reference, RedirectViewModel link, NotificationType type);
    }
}
