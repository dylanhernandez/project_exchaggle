using Exchaggle.Enumerations;
using Exchaggle.Helpers;
using Exchaggle.Models;
using Exchaggle.ViewModels.Common;
using Exchaggle.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Exchaggle.Services
{
    public class SearchService
    {
        private ExchaggleDbContext db;
        private GlobalService global;
        private ImageService imaging;

        public SearchService(ExchaggleDbContext databaseContext)
        {
            db = databaseContext;
            global = new GlobalService(db);
            imaging = new ImageService(db);
        }

        public SearchFormViewModel NewSearchForm()
        {
            string emailAddress = HttpContext.Current.User.Identity.Name.ToString();
            if (!global.IsUniqueEmailAddress(emailAddress))
            {
                Account userAccount = global.GetAccount(emailAddress);
                SearchFormViewModel newForm = new SearchFormViewModel(userAccount.State);
                newForm.CategoriesList = global.GetCategoryList();
                newForm.City = userAccount.City;
                if (newForm.CategoriesList == null)
                {
                    return null;
                }
                newForm.SubcategoriesList = global.GetSubCategoryList();
                return newForm;
            }
            return null;
        }

        public SearchResultsViewModel Search(SearchFormViewModel queryForm)
        {
            SearchResultsViewModel returnForm = new SearchResultsViewModel();
            returnForm.SearchQuery = queryForm.SearchQuery;
            returnForm.SearchCategory = queryForm.SearchCategory;
            returnForm.SearchSubcategory = queryForm.SearchSubcategory;
            returnForm.TradeCategory = queryForm.TradeCategory;
            returnForm.TradeSubcategory = queryForm.TradeSubcategory;
            returnForm.State = queryForm.State;
            returnForm.City = queryForm.City;
            returnForm.Results = GetSearchResults(returnForm, 0);
            return returnForm;
        }

        public SearchResultsViewModel Search(SearchResultsViewModel queryForm, int page)
        {
            SearchResultsViewModel returnForm = new SearchResultsViewModel();
            returnForm.SearchQuery = queryForm.SearchQuery;
            returnForm.SearchCategory = queryForm.SearchCategory;
            returnForm.SearchSubcategory = queryForm.SearchSubcategory;
            returnForm.TradeCategory = queryForm.TradeCategory;
            returnForm.TradeSubcategory = queryForm.TradeSubcategory;
            returnForm.State = queryForm.State;
            returnForm.City = queryForm.City;
            returnForm.Results = GetSearchResults(returnForm, page);
            return returnForm;
        }

        private IEnumerable<SearchResultItemViewModel> GetSearchResults(SearchResultsViewModel paramForm, int page)
        {
            const int PAGE_SIZE = ConstHelper.CONST_PAGE_SIZE;
            string emailAddress = HttpContext.Current.User.Identity.Name.ToString();
            if (!global.IsUniqueEmailAddress(emailAddress))
            {
                Account userAccount = global.GetAccount(emailAddress);

                int searchCategory = paramForm.SearchCategory;
                int searchSubCategory = paramForm.SearchSubcategory;
                int tradeCategory = paramForm.TradeCategory;
                int tradeSubCategory = paramForm.TradeSubcategory;

                var itemResults = (searchCategory > 0)
                    ? db.Item
                    .Join(db.ItemDetail, it => it.ItemId, itd => itd.ItemId, (it, itd) => new { it, itd })
                    .Join(db.Account, ai => ai.it.AccountId, ac => ac.AccountId, (ai, ac) => new { ai, ac })
                    .OrderByDescending(od => od.ai.it.CategoryId == ((searchCategory > 0) ? searchCategory : -1))
                    .ThenBy(od => od.ai.it.SubcategoryId == ((searchSubCategory > 0) ? searchSubCategory : -1))
                    .ThenBy(od => od.ai.it.TradeCategoryId == ((tradeCategory > 0) ? tradeCategory : -1))
                    .ThenBy(od => od.ai.it.TradeSubcategoryId == ((tradeSubCategory > 0) ? tradeSubCategory : -1))
                    .ThenBy(od => od.ai.itd.UploadDate)
                    .ThenBy(od => od.ac.Country == userAccount.Country)
                    .ThenBy(od => od.ac.State == paramForm.State)
                    .ThenBy(od => od.ac.State == userAccount.State)
                    .ThenBy(od => od.ac.City == paramForm.City)
                    .ThenBy(od => od.ac.City == userAccount.City)
                    .Where(i => (i.ai.it.Name.ToLower().Contains(paramForm.SearchQuery.ToLower())
                    || i.ai.it.Caption.ToLower().Contains(paramForm.SearchQuery.ToLower())
                    || i.ai.it.Description.ToLower().Contains(paramForm.SearchQuery.ToLower()))
                    && i.ai.itd.ItemStatus == (int)ItemStatusType.Normal).
                    Skip(page * PAGE_SIZE).Take(PAGE_SIZE).ToList()
                    : db.Item
                    .Join(db.ItemDetail, it => it.ItemId, itd => itd.ItemId, (it, itd) => new { it, itd })
                    .Join(db.Account, ai => ai.it.AccountId, ac => ac.AccountId, (ai, ac) => new { ai, ac })
                    .OrderByDescending(od => od.ai.it.TradeCategoryId == ((tradeCategory > 0) ? tradeCategory : -1))
                    .ThenBy(od => od.ai.it.TradeSubcategoryId == ((tradeSubCategory > 0) ? tradeSubCategory : -1))
                    .ThenBy(od => od.ai.it.CategoryId == ((searchCategory > 0) ? searchCategory : -1))
                    .ThenBy(od => od.ai.it.SubcategoryId == ((searchSubCategory > 0) ? searchSubCategory : -1))
                    .ThenBy(od => od.ai.itd.UploadDate)
                    .ThenBy(od => od.ac.Country == userAccount.Country)
                    .ThenBy(od => od.ac.State == paramForm.State)
                    .ThenBy(od => od.ac.State == userAccount.State)
                    .ThenBy(od => od.ac.City == paramForm.City)
                    .ThenBy(od => od.ac.City == userAccount.City)
                    .Where(i => (i.ai.it.Name.ToLower().Contains(paramForm.SearchQuery.ToLower())
                    || i.ai.it.Caption.ToLower().Contains(paramForm.SearchQuery.ToLower())
                    || i.ai.it.Description.ToLower().Contains(paramForm.SearchQuery.ToLower()))
                    && i.ai.itd.ItemStatus == (int)ItemStatusType.Normal).
                    Skip(page * PAGE_SIZE).Take(PAGE_SIZE).ToList();

                if (itemResults.Count > 0)
                {
                    List<SearchResultItemViewModel> resultsList = new List<SearchResultItemViewModel>();
                    for (int x = 0; x < itemResults.Count; x++)
                    {
                        SearchResultItemViewModel newItem = new SearchResultItemViewModel();
                        newItem.ItemId = itemResults[x].ai.it.ItemId;
                        newItem.Name = itemResults[x].ai.it.Name;
                        newItem.Caption = itemResults[x].ai.it.Caption;
                        newItem.Description = itemResults[x].ai.it.Description;
                        newItem.IsYours = (userAccount.AccountId == itemResults[x].ai.it.AccountId) ? true : false;
                        Image itemImage = imaging.ServeImage(itemResults[x].ai.it.ItemId);
                        if (itemImage != null)
                        {
                            newItem.ImageSource = itemImage.ImageSource;
                        }
                        resultsList.Add(newItem);
                    }
                    return resultsList;
                }
            }
            return null;
        }

        public IEnumerable<ReportableResultViewModel> getReportedResults(ReportType type, string search)
        {
            List<ReportableResultViewModel> returnResults = new List<ReportableResultViewModel>();
            List<int> reportableIdCollection = new List<int>();
            if (type == ReportType.Item)
            {
                var itemResults = db.Item.Join(db.ItemDetail, it => it.ItemId, itd => itd.ItemId, (it, itd) => new { it, itd }).OrderByDescending(od => od.itd.UploadDate).Where(i => (i.it.Name.ToLower().Contains(search.ToLower()) || i.it.Caption.ToLower().Contains(search.ToLower()) || i.it.Description.ToLower().Contains(search.ToLower())) && i.itd.Reported == 1).ToList();
                foreach (var res in itemResults)
                {
                    reportableIdCollection.Add(res.it.ItemId);
                }
            }
            else
            {
                var accountResults = db.Account.Join(db.AccountDetail, ac => ac.AccountId, acd => acd.AccountId, (ac, acd) => new { ac, acd }).Where(a => (a.ac.EmailAddress.ToLower().Contains(search.ToLower()) || a.ac.Username.ToLower().Contains(search.ToLower())) && a.acd.AccountStatus == (int)AccountStatusType.Reported).ToList();
                foreach (var res in accountResults)
                {
                    reportableIdCollection.Add(res.ac.AccountId);
                }
            }
            List<Report> results = db.Report.Where(r => r.ReportableType == (int)type && reportableIdCollection.Contains(r.ReportableId)).ToList();
            if (results != null)
            {
                foreach (Report r in results)
                {
                    ReportableResultViewModel reportModel = new ReportableResultViewModel();
                    reportModel.ReportId = r.ReportId;
                    reportModel.Details = r.Description;
                    if (type == ReportType.Account)
                    {
                        Account reportedAccount = db.Account.Where(a => a.AccountId == r.ReportableId).FirstOrDefault();
                        if (reportedAccount == null)
                        {
                            continue;
                        }
                        reportModel.ReferenceId = reportedAccount.AccountId;
                        reportModel.ReferenceName = reportedAccount.EmailAddress;
                        reportModel.PostedName = reportedAccount.Username;
                    }
                    else
                    {
                        Item reportedItem = db.Item.Where(i => i.ItemId == r.ReportableId).FirstOrDefault();
                        if (reportedItem == null)
                        {
                            continue;
                        }
                        Account reportedItemAccount = db.Account.Where(a => a.AccountId == reportedItem.AccountId).FirstOrDefault();
                        if (reportedItemAccount == null)
                        {
                            continue;
                        }
                        reportModel.ReferenceId = reportedItem.ItemId;
                        reportModel.ReferenceName = reportedItem.Name;
                        reportModel.PostedName = reportedItemAccount.Username;
                    }
                    returnResults.Add(reportModel);
                }
                return returnResults;
            }
            return null;
        }
    }
}