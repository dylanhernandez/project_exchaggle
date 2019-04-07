using Exchaggle.Attributes;
using Exchaggle.Models;
using Exchaggle.Services;
using Exchaggle.ViewModels;
using Exchaggle.ViewModels.Settings;
using System.Web.Mvc;

namespace Exchaggle.Controllers
{
    [AccessAuthorization]
    public class SettingsController : Controller
    {
        private ExchaggleDbContext dataContext;
        private GlobalService global;
        private SettingService settingsManager;
        private AdminService adminManager;

        public SettingsController()
        {
            dataContext = new ExchaggleDbContext();
            settingsManager = new SettingService(dataContext);
            global = new GlobalService(dataContext);
            adminManager = new AdminService(dataContext);
        }

        public ActionResult Index()
        {
            bool check = adminManager.CheckAccountLevel();
            if (check)
            {
                ViewBag.Access = 1;
            }
            else
            {
                ViewBag.Access = 0;
            }
            return View();
        }

        public ActionResult ChangeUsername()
        {
            SessionToViewBag();
            return View();
        }

        public ActionResult ChangeEmail()
        {
            SessionToViewBag();
            return View();
        }

        public ActionResult ChangePassword()
        {
            SessionToViewBag();
            return View();
        }

        public ActionResult ChangeContactInfo()
        {
            SessionToViewBag();
            string emailAddress = HttpContext.User.Identity.Name.ToString();
            Account referenceAccount = global.GetAccount(emailAddress);
            SettingContactViewModel baseModel = (referenceAccount != null) ? new SettingContactViewModel(referenceAccount.State) : new SettingContactViewModel();
            return View(baseModel);
        }

        public ActionResult CancelAccount()
        {
            SessionToViewBag();
            return View();
        }

        public ActionResult About()
        {
            SessionToViewBag();
            return View();
        }

        public ActionResult Admin()
        {
            bool check = adminManager.CheckAccountLevel();
            if (check)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Settings");
            }
        }

        [HttpPost]
        public ActionResult ChangeUsername(SettingUsernameViewModel postForm)
        {
            string response;
            bool change = settingsManager.ChangeUsername(postForm, out response);
            if (change)
            {
                Session["updateMessage"] = response;
            }
            else
            {
                Session["errorMessage"] = response;
            }
            return RedirectToAction("ChangeUsername", "Settings");
        }

        [HttpPost]
        public ActionResult ChangeEmail(SettingEmailViewModel postForm)
        {
            string response;
            bool change = settingsManager.ChangeEmail(postForm, out response);
            if (change)
            {
                Session["updateMessage"] = response;
                return RedirectToAction("Logout", "Account");
            }
            else
            {
                Session["errorMessage"] = response;
                return RedirectToAction("ChangeEmail", "Settings");
            }
        }

        [HttpPost]
        public ActionResult ChangePassword(SettingPasswordViewModel postForm)
        {
            string response;
            bool change = settingsManager.ChangePassword(postForm, out response);
            if (change)
            {
                Session["updateMessage"] = response;
                return RedirectToAction("Logout", "Account");
            }
            else
            {
                Session["errorMessage"] = response;
                return RedirectToAction("ChangePassword", "Settings");
            }
        }

        [HttpPost]
        public ActionResult ChangeContactInfo(SettingContactViewModel postForm)
        {
            string response;
            bool change = settingsManager.ChangeContactInformation(postForm, out response);
            if (change)
            {
                Session["updateMessage"] = response;
            }
            else
            {
                Session["errorMessage"] = response;
            }
            return RedirectToAction("ChangeContactInfo", "Settings");
        }

        [HttpPost]
        public ActionResult CancelAccount(LoginViewModel postForm)
        {
            string response;
            bool change = settingsManager.CancelAccount(postForm, out response);
            if (change)
            {
                Session["updateMessage"] = response;
                return RedirectToAction("Logout", "Account");
            }
            else
            {
                Session["errorMessage"] = response;
                return RedirectToAction("CancelAccount", "Settings");
            }
        }

        private void SessionToViewBag()
        {
            if (Session["updateMessage"] != null)
            {
                string postMessage = Session["updateMessage"].ToString();
                Session.Remove("updateMessage");
                ViewBag.Message = postMessage;
            }
            if (Session["errorMessage"] != null)
            {
                string postMessage = Session["errorMessage"].ToString();
                Session.Remove("errorMessage");
                ViewBag.ErrorMessage = postMessage;
            }
        }
    }
}