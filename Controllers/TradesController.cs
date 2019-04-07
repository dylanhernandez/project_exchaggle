using Exchaggle.Attributes;
using Exchaggle.Enumerations;
using Exchaggle.Models;
using Exchaggle.Services;
using Exchaggle.ViewModels.Trades;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Exchaggle.Controllers
{
    [AccessAuthorization]
    public class TradesController : Controller
    {
        private ExchaggleDbContext dataContext;
        private TradeService tradeManager;
        private GlobalService global;

        public TradesController()
        {
            dataContext = new ExchaggleDbContext();
            tradeManager = new TradeService(dataContext);
            global = new GlobalService(dataContext);
        }

        public ActionResult Index()
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
            if (Session["currentSearch"] != null)
            {
                Session.Remove("currentSearch");
            }
            IEnumerable<OfferBulletinViewModel> myOffers = tradeManager.OfferList(OfferType.Sender);
            return View(myOffers);
        }

        public ActionResult UserOffers(int reference = -1)
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
            if (Session["currentSearch"] != null)
            {
                Session.Remove("currentSearch");
            }
            IEnumerable<OfferBulletinViewModel> userOffers = tradeManager.OfferList(OfferType.Receiver, reference);
            return View(userOffers);
        }

        public ActionResult Details(int referenceA, int referenceB)
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
            TradesDetailViewModel tradeDetails = tradeManager.GetTradeDetails(referenceA, referenceB);
            if (tradeDetails != null)
            {
                return View(tradeDetails);
            }
            return RedirectToAction("Index", "Trades"); //Proper error message
        }

        public ActionResult MakeOffer(int reference)
        {
            if (reference == 0)
            {
                return RedirectToAction("Index", "Trades"); //Need to insert proper error message
            }
            MakeOfferViewModel makeOfferForm = tradeManager.CreateOfferList(reference);
            if (makeOfferForm == null)
            {
                return RedirectToAction("Index", "Trades");
            }
            return View(makeOfferForm);
        }

        public ActionResult MakeOfferConfirm(int referenceA, int referenceB)
        {
            if (referenceA == 0 || referenceB == 0)
            {
                return RedirectToAction("Index", "Trades"); //Need to insert proper error message
            }            
            OfferBulletinViewModel makeOfferConfirm = tradeManager.CreateOfferPrompt(referenceA, referenceB);
            if (makeOfferConfirm == null)
            {
                return RedirectToAction("Index", "Trades");
            }
            return View(makeOfferConfirm);
        }

        public ActionResult CreateOffer(int referenceA, int referenceB)
        {
            bool create = tradeManager.CreateOffer(referenceA, referenceB);
            if (create)
            {
                string responseMessage = "Success! New offer has been created.";
                Session["updateMessage"] = responseMessage;
            }
            return RedirectToAction("Index", "Trades");
        }

        public ActionResult DeleteOffer(int referenceA, int referenceB)
        {
            string responseMessage;
            bool delete = tradeManager.DeleteOffer(referenceA, referenceB, out responseMessage);
            if (delete)
            {
                Session.Remove("reference");
                Session["updateMessage"] = responseMessage;
            }
            else
            {
                Session.Remove("reference");
                Session["errorMessage"] = responseMessage;
            }
            return RedirectToAction("Index", "Trades");
        }

        public ActionResult RejectOffer(int referenceA, int referenceB)
        {
            string responseMessage;
            bool delete = tradeManager.DeleteOffer(referenceA, referenceB, out responseMessage);
            if (delete)
            {
                Session.Remove("reference");
                Session["updateMessage"] = responseMessage;
            }
            else
            {
                Session.Remove("reference");
                Session["errorMessage"] = responseMessage;
            }
            return RedirectToAction("UserOffers", "Trades");
        }

        public ActionResult ConfirmOffer(int referenceA, int referenceB)
        {
            string responseMessage;
            bool confirm = tradeManager.ConfirmOffer(referenceA, referenceB, out responseMessage);
            if (confirm)
            {
                Session.Remove("reference");
                Session["updateMessage"] = responseMessage;
            }
            else
            {
                Session.Remove("reference");
                Session["errorMessage"] = responseMessage;
            }
            return RedirectToAction("Details", "Trades", new { referenceA = referenceA, referenceB = referenceB});
        }
    }
}