using Exchaggle.Attributes;
using Exchaggle.Models;
using Exchaggle.Services;
using Exchaggle.ViewModels.Home;
using System.Web.Mvc;

namespace Exchaggle.Controllers
{
    public class HomeController : Controller
    {
        [AccessAuthorization]
        public ActionResult Index()
        {
            DashboardService dashboardManager = new DashboardService(new ExchaggleDbContext());
            DashboardViewModel dashboard;
            dashboard = dashboardManager.CreateDashboard();
            if (Session["updateMessage"] != null)
            {
                string postMessage = Session["updateMessage"].ToString();
                Session.Remove("updateMessage");
                ViewBag.Message = postMessage;
            }
            return View(dashboard);
        }

        public ActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}