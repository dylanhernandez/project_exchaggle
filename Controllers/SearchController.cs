using Exchaggle.Attributes;
using Exchaggle.Models;
using Exchaggle.Services;
using Exchaggle.ViewModels.Common;
using Exchaggle.ViewModels.Search;
using System.Web.Mvc;

namespace Exchaggle.Controllers
{
    [AccessAuthorization]
    public class SearchController : Controller
    {
        private ExchaggleDbContext dataContext;
        private SearchService searchFormManager;
        private GlobalService global;

        public SearchController()
        {
            dataContext = new ExchaggleDbContext();
            searchFormManager = new SearchService(dataContext);
            global = new GlobalService(dataContext);
        }

        public ActionResult Index()
        {
            if (Session["currentSearch"] != null)
            {
                Session.Remove("currentSearch");
            }
            SearchFormViewModel searchForm = searchFormManager.NewSearchForm();
            return View(searchForm);
        }

        public ActionResult Result(int reference)
        {
            SearchResultsViewModel currentSearch;
            if (Session["currentSearch"] != null)
            {
                currentSearch = Session["currentSearch"] as SearchResultsViewModel;
                if (reference == -1)
                {
                    reference = currentSearch.Page;
                }
                SearchResultsViewModel updatedSearch = searchFormManager.Search(currentSearch, reference);
                if (updatedSearch != null)
                {
                    updatedSearch.Page = reference;
                    Session.Remove("currentSearch");
                    Session["currentSearch"] = updatedSearch;
                    return View(updatedSearch);
                }

            }
            return RedirectToAction("Index", "Search");
        }

        public ActionResult Details(int reference)
        {
            ItemsDetailViewModel detailItem = global.GetItemDetail(reference);
            if (detailItem != null)
            {
                if (Session["errorMessage"] != null)
                {
                    string postMessage = Session["errorMessage"].ToString();
                    Session.Remove("errorMessage");
                    ViewBag.ErrorMessage = postMessage;
                }
                if (Session["updateMessage"] != null)
                {
                    string postMessage = Session["updateMessage"].ToString();
                    Session.Remove("updateMessage");
                    ViewBag.UpdateMessage = postMessage;
                }
                if (Session["currentSearch"] != null)
                {
                    SearchResultsViewModel currentSearch = Session["currentSearch"] as SearchResultsViewModel;
                    ViewBag.ReturnUrl = "/Search/Result/" + currentSearch.Page;
                }
                return View(detailItem);
            }
            return RedirectToAction("Index", "Search");
        }

        [HttpPost]
        public ActionResult Result(SearchFormViewModel searchForm)
        {
            SearchResultsViewModel results = searchFormManager.Search(searchForm);
            Session["currentSearch"] = results;
            return View(results);
        }
    }
}