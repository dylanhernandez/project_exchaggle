using Exchaggle.Enumerations;
using Exchaggle.Models;
using Exchaggle.Services;
using System.Web.Mvc;

namespace Exchaggle.Controllers
{
    [Authorize]
    public class AjaxController : Controller
    {
        private ExchaggleDbContext dataContext;
        private GlobalService global;
        private DashboardService dashboardManager;
        private SearchService searchManager;
        private ReportService reportManager;

        public AjaxController()
        {
            dataContext = new ExchaggleDbContext();
            global = new GlobalService(dataContext);
            dashboardManager = new DashboardService(dataContext);
            searchManager = new SearchService(dataContext);
            reportManager = new ReportService(dataContext);
        }

        [HttpPost]
        public ActionResult PresetCategoryJson(int reference)
        {
            object results = global.GetPresets(reference);
            return Json(results);
        }

        [HttpPost]
        public ActionResult SubcategoriesJson(int reference)
        {
            object subCategories = global.GetSubCategoryList(reference);
            return Json(subCategories);
        }

        [HttpPost]
        public ActionResult NotificationClear(int reference)
        {
            bool isCleared = dashboardManager.ClearNotification(reference);
            return Json(isCleared);
        }

        [HttpPost]
        public ActionResult SearchReportedItems(string search)
        {
            object results = searchManager.getReportedResults(ReportType.Item, search);
            return Json(results);
        }

        [HttpPost]
        public ActionResult SearchReportedAccounts(string search)
        {
            object results = searchManager.getReportedResults(ReportType.Account, search);
            return Json(results);
        }

        [HttpPost]
        public ActionResult ReportClearStatus(int reportId)
        {
            int result = reportManager.ClearReportStatus(reportId);
            return Json(result);
        }

        [HttpPost]
        public ActionResult ReportRemoveObject(int reportId)
        {
            int result = reportManager.RemoveReportable(reportId);
            return Json(result);
        }
    }
}