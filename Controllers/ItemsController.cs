using Exchaggle.Attributes;
using Exchaggle.Models;
using Exchaggle.Services;
using Exchaggle.ViewModels.Common;
using Exchaggle.ViewModels.Items;
using Exchaggle.ViewModels.Search;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Exchaggle.Controllers
{
    [AccessAuthorization]
    public class ItemsController : Controller
    {
        private ExchaggleDbContext dataContext;
        private UploadService uploadManager;
        private ItemService itemManager;
        private GlobalService global;

        public ItemsController()
        {
            dataContext = new ExchaggleDbContext();
            uploadManager = new UploadService(dataContext);
            itemManager = new ItemService(dataContext);
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
            if (Session["reference"] != null)
            {
                Session.Remove("reference");
            }
            if (Session["currentSearch"] != null)
            {
                Session.Remove("currentSearch");
            }
            IEnumerable<ItemBulletinViewModel> userItems = itemManager.GetItemList();
            return View(userItems);
        }

        public ActionResult Wishlist()
        {
            if (Session["currentSearch"] != null)
            {
                Session.Remove("currentSearch");
            }
            IEnumerable<ItemBulletinViewModel> wishlist = itemManager.GetWishList();
            return View(wishlist);
        }

        public ActionResult Upload()
        {
            UploadFormViewModel newForm = uploadManager.NewUploadForm();
            if (Session["updateMessage"] != null)
            {
                var postMessage = Session["updateMessage"];
                Session.Remove("updateMessage");
                ViewBag.Collection = postMessage;
            }
            if (uploadManager.isItemCollectionFull())
            {
                Session["errorMessage"] = Resources.Processing.ProcessMaxItems;
                return RedirectToAction("Index", "Items");
            }
            return View(newForm);
        }

        public ActionResult Edit(int reference)
        {
            UploadFormViewModel newForm = uploadManager.EditUploadForm(reference);
            if (Session["reference"] != null)
            {
                Session.Remove("reference");
            }
            if (Session["updateMessage"] != null)
            {
                var postMessage = Session["updateMessage"];
                Session.Remove("updateMessage");
                ViewBag.Collection = postMessage;
            }
            if (newForm == null)
            {
                Session["errorMessage"] = Resources.Processing.ProcessEditNoForm;
                return RedirectToAction("Index", "Items");
            }
            Session["reference"] = reference;
            return View("Upload",newForm);
        }

        public ActionResult Delete(int reference)
        {
            string responseMessage;
            bool deleteItem = itemManager.DeleteItem(reference, out responseMessage);
            if (deleteItem)
            {
                Session.Remove("reference");
                Session["updateMessage"] = responseMessage;
            }
            return RedirectToAction("Index", "Items");
        }

        public ActionResult Details(int reference)
        {
            ItemsDetailViewModel detailItem = global.GetItemDetail(reference);
            if (detailItem != null)
            {
                if (Session["currentSearch"] != null)
                {
                    SearchResultsViewModel currentSearch = Session["currentSearch"] as SearchResultsViewModel;
                    ViewBag.ReturnUrl = "/Search/Result/" + currentSearch.Page;
                }
                return View(detailItem);
            }
            Session["errorMessage"] = Resources.Processing.ProcessEditNoForm;
            return RedirectToAction("Index", "Items");
        }

        public ActionResult AddToWishlist(int reference)
        {
            bool isAdded = global.CheckWishList(reference);
            if (!isAdded)
            {
                string responseMessage;
                bool addToWishList = itemManager.AppendWishlist(reference, out responseMessage);
                if (addToWishList)
                {
                    Session["updateMessage"] = responseMessage;
                }
                else
                {
                    Session["errorMessage"] = responseMessage;
                }
                return RedirectToAction("Details", "Search", new { reference = reference });
            }
            Session["errorMessage"] = Resources.Processing.ProcessError;
            return RedirectToAction("Details", "Search", new { reference = reference });
        }

        public ActionResult RemoveFromWishlist(int reference)
        {
            bool isAdded = global.CheckWishList(reference);
            if (isAdded)
            {
                string responseMessage;
                bool removeFromWishlist = itemManager.DetachWishlist(reference, out responseMessage);
                if (removeFromWishlist)
                {
                    Session["updateMessage"] = responseMessage;
                }
                else
                {
                    Session["errorMessage"] = responseMessage;
                }
                return RedirectToAction("Details", "Search", new { reference = reference });
            }
            Session["errorMessage"] = Resources.Processing.ProcessError;
            return RedirectToAction("Details", "Search", new { reference = reference });
        }

        [HttpPost]
        public ActionResult Upload(UploadFormViewModel postForm)
        {
            var validationOutputs = new string[0];
            bool validForm = uploadManager.ValidateUploadForm(postForm, out validationOutputs);
            if (validForm)
            {
                string responseMessage;
                bool addItem = uploadManager.CreateUploadItem(postForm, out responseMessage);
                if (addItem)
                {
                    Session["updateMessage"] = responseMessage;
                    return RedirectToAction("Index", "Items");
                }
                else
                {
                    Session["updateMessage"] = new string[] { responseMessage };
                    return RedirectToAction("Upload", "Items");
                }
            }
            else
            {
                Session["updateMessage"] = validationOutputs;
                return RedirectToAction("Upload", "Items");
            }
        }

        [HttpPost]
        public ActionResult Edit(UploadFormViewModel postForm)
        {
            var validationOutputs = new string[0];
            bool validForm = uploadManager.ValidateUploadForm(postForm, out validationOutputs);
            if (validForm)
            {
                string responseMessage;
                postForm.ReferenceId = int.Parse(Session["reference"].ToString());
                bool editItem = uploadManager.EditUploadItem(postForm, out responseMessage);
                if (editItem)
                {
                    Session["updateMessage"] = responseMessage;
                    return RedirectToAction("Index", "Items");
                }
                else
                {
                    Session["updateMessage"] = new string[] { responseMessage };
                    return RedirectToAction("Edit", "Items", new { reference = int.Parse(Session["reference"].ToString()) });
                }
            }
            else
            {
                Session["updateMessage"] = validationOutputs;
                return RedirectToAction("Edit", "Items", new { reference = int.Parse(Session["reference"].ToString()) });
            }
        }

    }
}