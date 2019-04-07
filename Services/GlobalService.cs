using Exchaggle.Models;
using Exchaggle.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Exchaggle.Services
{
    public class GlobalService
    {
        private ExchaggleDbContext db;
        private ImageService imaging;

        public GlobalService(ExchaggleDbContext databaseContext)
        {
            db = databaseContext;
            imaging = new ImageService(db);
        }

        public bool IsUniqueEmailAddress(string emailAddress)
        {
            var checkAccount = db.Account.Where(c => c.EmailAddress == emailAddress.ToLower()).FirstOrDefault();
            if (checkAccount != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool IsUniqueUsername(string userName)
        {
            var checkAccount = db.Account.Where(c => c.Username.ToLower() == userName.ToLower()).FirstOrDefault();
            if (checkAccount != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Account GetAccount(string email)
        {
            Account findAccount = db.Account.Where(c => c.EmailAddress == email).FirstOrDefault();
            return (findAccount != null) ? findAccount : null;
        }

        public SelectList GetCategoryList(int startingItem = 0)
        {
            IEnumerable<Category> categoryCollection = db.Category.ToList();
            List<object> categorySelectList = new List<object>();
            categorySelectList.Add(new { value = 0, text = "All / Anything" });
            if (categoryCollection != null)
            {
                foreach (Category c in categoryCollection)
                {
                    categorySelectList.Add(new { value = c.CategoryId, text = c.Name });
                }
            }
            SelectList returnList = new SelectList(categorySelectList, "value", "text", startingItem);
            return returnList;
        }

        public SelectList GetSubCategoryList(int categoryId = 0)
        {
            IEnumerable<Subcategory> subategoryCollection = db.Subcategory.Where(c => c.CategoryId == categoryId).ToList();
            List<object> subcategorySelectList = new List<object>();
            subcategorySelectList.Add(new { value = 0, text = "All / Anything" });
            if (subategoryCollection != null)
            {
                foreach (Subcategory sc in subategoryCollection)
                {
                    subcategorySelectList.Add(new { value = sc.SubcategoryId, text = sc.Name });
                }
            }
            SelectList returnList = new SelectList(subcategorySelectList, "value", "text", 0);
            return returnList;
        }

        public ItemsDetailViewModel GetItemDetail(int itemId)
        {
            if (itemId > 0)
            {
                Item requestItem = db.Item.Where(i => i.ItemId == itemId).FirstOrDefault();
                if (requestItem != null)
                {
                    string emailAddress = HttpContext.Current.User.Identity.Name.ToString();
                    ItemsDetailViewModel itemDetails = new ItemsDetailViewModel();
                    Account postedAccount = db.Account.Where(a => a.AccountId == requestItem.AccountId).FirstOrDefault();
                    if (postedAccount == null)
                    {
                        return null;
                    }

                    bool check = isConfirmedItem(requestItem);

                    itemDetails.ItemId = requestItem.ItemId;
                    itemDetails.Name = requestItem.Name;
                    itemDetails.Caption = requestItem.Caption;
                    itemDetails.Description = requestItem.Description;
                    itemDetails.PostedBy = (postedAccount.EmailAddress.ToLower() == emailAddress.ToLower()) ? "You" : postedAccount.Username;
                    itemDetails.InWishList = CheckWishList(requestItem.ItemId);
                    Image itemImage = imaging.ServeImage(requestItem);
                    if (itemImage != null)
                    {
                        itemDetails.ImageString = itemImage.ImageSource;
                    }
                    if (check)
                    {
                        itemDetails.Status = 1;
                    }

                    Category itemCategory = db.Category.Where(c => c.CategoryId == requestItem.CategoryId).FirstOrDefault();
                    Subcategory itemSubcategory = db.Subcategory.Where(sc => sc.SubcategoryId == requestItem.SubcategoryId).FirstOrDefault();
                    itemDetails.CategoryName = (itemCategory != null) ? itemCategory.Name : "None";
                    itemDetails.SubcategoryName = (itemSubcategory != null) ? itemSubcategory.Name : "Anything";

                    string tradeCategoryTitle = "All";
                    string tradeSubcategoryTitle = "Anything";
                    if (requestItem.TradeCategoryId > 0)
                    {
                        Category tradeCategory = db.Category.Where(c => c.CategoryId == requestItem.TradeCategoryId).FirstOrDefault();
                        tradeCategoryTitle = (tradeCategory != null) ? tradeCategory.Name : tradeCategoryTitle;
                    }
                    if (requestItem.TradeSubcategoryId > 0)
                    {
                        Subcategory tradeSubcategory = db.Subcategory.Where(c => c.SubcategoryId == requestItem.TradeSubcategoryId).FirstOrDefault();
                        tradeSubcategoryTitle = (tradeSubcategory != null) ? tradeSubcategory.Name : tradeSubcategoryTitle;
                    }
                    itemDetails.TradeCategoryName = tradeCategoryTitle;
                    itemDetails.TradeSubcategoryName = tradeSubcategoryTitle;
                    return itemDetails;
                }
            }
            return null;
        }

        public object GetPresets(int itemId)
        {
            Item refItem = db.Item.Where(i => i.ItemId == itemId).FirstOrDefault();
            if (refItem != null)
            {
                int[] presets = new int[] { refItem.CategoryId, refItem.SubcategoryId, refItem.TradeCategoryId, refItem.TradeSubcategoryId };
                return presets;
            }
            return null;
        }

        public bool CheckWishList(int itemId)
        {
            string emailAddress = HttpContext.Current.User.Identity.Name.ToString();
            if (!IsUniqueEmailAddress(emailAddress))
            {
                try
                {
                    Account userAccount = GetAccount(emailAddress);
                    Wishlist referenceItem = db.Wishlist.Where(w => w.AccountId == userAccount.AccountId && w.ItemId == itemId).FirstOrDefault();
                    if (referenceItem != null)
                    {
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        public bool isConfirmedItem(Item item)
        {
            List<Offer> checkOffers = db.Offer.Where(o => o.SenderItemId == item.ItemId || o.ReceiverItemId == item.ItemId).ToList();
            if (checkOffers != null)
            {
                foreach (Offer offer in checkOffers)
                {
                    OfferDetail checkDetails = db.OfferDetail.Where(od => od.OfferId == offer.OfferId).FirstOrDefault();
                    if (checkDetails != null)
                    {
                        if (checkDetails.Confirmed > 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}