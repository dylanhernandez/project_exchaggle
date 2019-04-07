using Exchaggle.Attributes;
using Exchaggle.Enumerations;
using Exchaggle.Models;
using Exchaggle.Services;
using Exchaggle.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Exchaggle.Controllers
{
    [AccessAuthorization]
    public class ReportController : Controller
    {
        private ExchaggleDbContext dataContext;
        private ReportService reporting;

        public ReportController()
        {
            dataContext = new ExchaggleDbContext();
            reporting = new ReportService(dataContext);
        }

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Item(int reference)
        {
            ReportViewModel reportSetup = reporting.GetReportSetup(reference, ReportType.Item);
            return View(reportSetup);
        }

        public ActionResult Account(int reference)
        {
            ReportViewModel reportSetup = reporting.GetReportSetup(reference, ReportType.Account);
            return View(reportSetup);
        }

        [HttpPost]
        public ActionResult Item(ReportViewModel postModel)
        {
            string response;
            bool runReport = reporting.SendReport(postModel, ReportType.Item, out response);
            Session["updateMessage"] = response;
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Account(ReportViewModel postModel)
        {
            string response;
            bool runReport = reporting.SendReport(postModel, ReportType.Account, out response);
            Session["updateMessage"] = response;
            return RedirectToAction("Index", "Home");
        }
    }
}