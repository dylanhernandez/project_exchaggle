using System.Web.Mvc;
using Exchaggle.Models;
using Exchaggle.Services;
using Exchaggle.ViewModels;

namespace Exchaggle.Controllers
{
    public class AccountController : Controller
    {
        private AccountService accountService = new AccountService(new ExchaggleDbContext());

        /// <summary>
        /// Register GET
        /// </summary>
        /// <returns></returns>
        public ActionResult Register()
        {
            if (Session["updateMessage"] != null)
            {
                string response = Session["updateMessage"].ToString();
                ViewBag.ErrorBlock = response;
            }
            Session.Clear();
            RegisterViewModel register = new RegisterViewModel();
            return View(register);
        }

        /// <summary>
        /// Login GET
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            if (Session["accountChanged"] != null)
            {
                string accountChanged = Session["accountChanged"].ToString();
                ViewBag.Message = accountChanged;
            }
            if (Session["updateMessage"] != null)
            {
                string response = Session["updateMessage"].ToString();
                ViewBag.Message = response;
            } 
            Session.Clear();
            return View();
        }

        /// <summary>
        /// Logout GET
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Logout()
        {
            accountService.EgressAccount();
            return RedirectToAction("Login");
        }

        /// <summary>
        /// Register POST
        /// </summary>
        /// <param name="registerAccount"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Register(RegisterViewModel registerAccount)
        {
            if (ModelState.IsValid)
            {
                string response;
                if (accountService.CreateAccount(registerAccount, out response))
                {
                    ViewBag.Message = response;
                    return View("Login");
                }
                else
                {
                    ViewBag.ErrorBlock = response;
                    return View("Register");
                }
            }
            Session["updateMessage"] = "Unable to submit, invalid form data provided.";
            return RedirectToAction("Register");
        }

        /// <summary>
        /// Login POST
        /// </summary>
        /// <param name="loginAccount"></param>
        /// <returns></returns>
        [HttpPost][ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel loginAccount)
        {
            
            if (accountService.AccessAccount(loginAccount))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Message = "Email/Username or Password Invalid!";
            return View();
        }

        /// <summary>
        /// ForgotPassword GET
        /// </summary>
        /// <returns></returns>
        public ActionResult ForgotPassword()
        {
            Session.Remove("email");
            Session.Remove("postAnswerA");
            Session.Remove("postAnswerB");
            return View();
        }

        /// <summary>
        /// ForgotPassword POST
        /// </summary>
        /// <param name="recoverAccount"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ForgotPassword(PasswordRecoveryViewModel recoverAccount)
        {
            Session["email"] = recoverAccount.EmailAddress;
            return RedirectToAction("SecurityQuestions","Account");
        }

        /// <summary>
        /// SecurityQuestions GET
        /// </summary>
        /// <returns></returns>
        public ActionResult SecurityQuestions()
        {
            if (Session["email"] == null)
            {
                ViewBag.Mesage = "Recovery session invalid!";
                return View("ForgotPassword");
            }
            PasswordRecoveryViewModel securityAccount = new PasswordRecoveryViewModel();
            securityAccount.EmailAddress = Session["email"].ToString();
            securityAccount = accountService.FindAccountSecurityDetails(securityAccount);
            if (securityAccount != null)
            {
                if (Session["error"] != null)
                {
                    string errorFromAction = Session["error"].ToString();
                    Session.Remove("error");
                    ViewBag.Message = errorFromAction;
                }
                return View("SecurityQuestions", securityAccount);
            }
            ViewBag.Message = "Unable to locate security details";
            return View("ForgotPassword");
        }

        /// <summary>
        /// SecurityQuestions POST
        /// </summary>
        /// <param name="recoverAccount"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SecurityQuestions(PasswordRecoveryViewModel recoverAccount)
        {
            Session["postAnswerA"] = recoverAccount.SecurityAnswerA;
            Session["postAnswerB"] = recoverAccount.SecurityAnswerB;
            return RedirectToAction("PasswordReset", "Account");
        }

        /// <summary>
        /// PasswordReset GET
        /// </summary>
        /// <returns></returns>
        public ActionResult PasswordReset()
        {
            if (Session["email"] == null || Session["postAnswerA"] == null || Session["postAnswerB"] == null)
            {
                ViewBag.Mesage = "Recovery session invalid!";
                return View("ForgotPassword");
            }
            PasswordRecoveryViewModel validateAccount = new PasswordRecoveryViewModel();
            validateAccount.EmailAddress = Session["Email"].ToString();
            validateAccount.SecurityAnswerA = Session["postAnswerA"].ToString();
            validateAccount.SecurityAnswerB = Session["postAnswerB"].ToString();

            bool check = accountService.VerifyAccountSecurityDetails(validateAccount);
            if (check)
            {
                return View();
            }
            Session["error"] = "Incorrect information entered!";
            return RedirectToAction("SecurityQuestions","Account");
        }

        /// <summary>
        /// PasswordReset POST
        /// </summary>
        /// <param name="resetPassword"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PasswordReset(PasswordResetViewModel resetPassword)
        {
            if (Session["email"] == null)
            {
                ViewBag.Mesage = "Recovery session invalid!";
                return View("ForgotPassword");
            }
            resetPassword.EmailAddress = Session["email"].ToString();
            bool check = accountService.ChangeAccountPassword(resetPassword);
            if (check)
            {
                Session["accountChanged"] = "Account settings updated!";
                return RedirectToAction("Login", "Account");
            }
            ViewBag.Message = "An error occurred, please try again";
            return View();
        }
    }
}