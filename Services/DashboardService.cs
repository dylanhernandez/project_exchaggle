using Exchaggle.Models;
using Exchaggle.ViewModels.Common;
using Exchaggle.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Exchaggle.Services
{
    public class DashboardService
    {
        private ExchaggleDbContext db;
        private GlobalService global;

        public DashboardService(ExchaggleDbContext databaseContext)
        {
            db = databaseContext;
            global = new GlobalService(db);
        }

        public DashboardViewModel CreateDashboard()
        {
            string emailAddress = HttpContext.Current.User.Identity.Name.ToString();
            Account checkAccount = global.GetAccount(emailAddress);
            if (checkAccount != null)
            {
                DashboardViewModel newDashboard = new DashboardViewModel();
                newDashboard.Username = checkAccount.Username;
                newDashboard.Notifications = CreateNotificationList();
                return newDashboard;
            }
            return null;
        }

        public IEnumerable<NotificationViewModel> CreateNotificationList()
        {
            string emailAddress = HttpContext.Current.User.Identity.Name.ToString();
            Account checkAccount = global.GetAccount(emailAddress);
            if (checkAccount != null)
            {
                List<NotificationViewModel> buildList = new List<NotificationViewModel>();
                List<Notification> recordsList = db.Notification.OrderByDescending(n => n.UploadDate).Where(n => n.AccountId == checkAccount.AccountId).Take(5).ToList();
                foreach (Notification n in recordsList)
                {
                    NotificationViewModel noteView = new NotificationViewModel();
                    noteView.NotificationId = n.NotificationId;
                    noteView.Description = n.Description;
                    noteView.DateTime = n.UploadDate.ToString();
                    RedirectViewModel link = new RedirectViewModel(n.Action, n.Contorller, n.Reference);
                    noteView.Link = link;
                    buildList.Add(noteView);
                }
                return buildList;
            }
            return null;
        }

        public bool ClearNotification(int reference = 0)
        {
            string emailAddress = HttpContext.Current.User.Identity.Name.ToString();
            Account checkAccount = global.GetAccount(emailAddress);
            if (checkAccount != null)
            {
                if (reference == 0)
                {
                    db.Notification.RemoveRange(db.Notification.Where(n => n.AccountId == checkAccount.AccountId));
                    db.SaveChanges();
                    return true;
                }
                else
                {
                    db.Notification.Remove(db.Notification.Where(n => n.NotificationId == reference).First());
                    db.SaveChanges();
                    return true;
                }
            }
            return false;
        }
    }
}