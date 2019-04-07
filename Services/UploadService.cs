using Exchaggle.Interfaces;
using Exchaggle.Models;
using Exchaggle.ViewModels.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Exchaggle.ViewModels.Common;
using Exchaggle.Enumerations;
using Exchaggle.Helpers;

namespace Exchaggle.Services
{
    public class UploadService : INotifiable
    {
        private ExchaggleDbContext db;
        private GlobalService global;
        private ImageService imaging;

        public UploadService(ExchaggleDbContext databaseContext)
        {
            db = databaseContext;
            global = new GlobalService(db);
            imaging = new ImageService(db);
        }

        public UploadFormViewModel NewUploadForm()
        {
            string emailAddress = HttpContext.Current.User.Identity.Name.ToString();
            if (!global.IsUniqueEmailAddress(emailAddress))
            {
                UploadFormViewModel newForm = new UploadFormViewModel();
                newForm.CategoriesList = global.GetCategoryList();
                if (newForm.CategoriesList == null)
                {
                    return null;
                }
                newForm.SubcategoriesList = global.GetSubCategoryList();
                newForm.ReferenceAction = "Upload";
                newForm.ReferenceId = 0;
                return newForm;
            }
            return null;
        }

        public UploadFormViewModel EditUploadForm(int itemId)
        {
            string emailAddress = HttpContext.Current.User.Identity.Name.ToString();
            if (!global.IsUniqueEmailAddress(emailAddress))
            {
                Account userAccount = global.GetAccount(emailAddress);
                Item referenceItem = (userAccount != null) ? db.Item.Where(i => i.ItemId == itemId && i.AccountId == userAccount.AccountId).FirstOrDefault() : null;
                if (referenceItem != null)
                {
                    UploadFormViewModel newForm = new UploadFormViewModel();
                    newForm.Name = referenceItem.Name;
                    newForm.Caption = referenceItem.Caption;
                    newForm.Description = referenceItem.Description;
                    Image itemImage = imaging.ServeImage(referenceItem);
                    if (itemImage != null)
                    {
                        newForm.ImageString = itemImage.ImageSource;
                        newForm.ImageKeep = 1;
                    }
                    newForm.CategoriesList = global.GetCategoryList();
                    if (newForm.CategoriesList == null)
                    {
                        return null;
                    }
                    newForm.SubcategoriesList = global.GetSubCategoryList();
                    newForm.ReferenceAction = "Edit";
                    newForm.ReferenceId = itemId;
                    return newForm;
                }
            }
            return null;
        }

        public bool ValidateUploadForm(UploadFormViewModel newForm, out string[] outputMessages)
        {
            bool flag = true;
            List<string> outputs = new List<string>();
            string emailAddress = HttpContext.Current.User.Identity.Name.ToString();
            Account userAccount = global.GetAccount(emailAddress);

            if (string.IsNullOrWhiteSpace(newForm.Name))
            {
                flag = false;
                outputs.Add("The title of your upload cannot be blank");
            }
            if (string.IsNullOrWhiteSpace(newForm.Caption))
            {
                flag = false;
                outputs.Add("The caption of your item cannot be blank");
            }
            if (string.IsNullOrWhiteSpace(newForm.Description))
            {
                flag = false;
                outputs.Add("The description of your item cannot be blank");
            }
            if (newForm.ItemCategory == 0)
            {
                flag = false;
                outputs.Add("The category of your item cannot be set to 'All / Anything'");
            }
            if (newForm.ItemCategory == 0)
            {
                flag = false;
                outputs.Add("The subcategory of your item cannot be set to 'All / Anything'");
            }

            outputMessages = outputs.ToArray();
            return flag;
        }

        public bool isItemCollectionFull()
        {
            string emailAddress = HttpContext.Current.User.Identity.Name.ToString();
            Account userAccount = global.GetAccount(emailAddress);
            if (userAccount != null)
            {
                List<Item> userItemCollection = db.Item.Where(i => i.AccountId == userAccount.AccountId).ToList();
                List<int> userItemCollectionIds = new List<int>();
                foreach (Item i in userItemCollection)
                {
                    userItemCollectionIds.Add(i.ItemId);
                }
                List<ItemDetail> unconfirmedItemCollection = db.ItemDetail.Where(d => userItemCollectionIds.Contains(d.ItemId) && d.ItemStatus != (int)ItemStatusType.Confirmed).ToList();
                if (unconfirmedItemCollection.Count() >= ConstHelper.CONST_MAX_ITEMS)
                {
                    return true;
                }
            }
            return false;
        }

        public bool CreateUploadItem(UploadFormViewModel newForm, out string outputMessage)
        {
            string emailAddress = HttpContext.Current.User.Identity.Name.ToString();
            Account userAccount = global.GetAccount(emailAddress);
            try
            {
                Item uploadItem = new Item();
                uploadItem.AccountId = userAccount.AccountId;
                uploadItem.Name = newForm.Name;
                uploadItem.Caption = newForm.Caption;
                uploadItem.Description = newForm.Description;
                uploadItem.CategoryId = newForm.ItemCategory;
                uploadItem.SubcategoryId = newForm.ItemSubcategory;
                uploadItem.TradeCategoryId = newForm.TradeCategory;
                uploadItem.TradeSubcategoryId = newForm.TradeSubcategory;
                db.Item.Add(uploadItem);
                db.SaveChanges();

                ItemDetail uploadItemDetail = new ItemDetail();
                uploadItemDetail.ItemId = uploadItem.ItemId;
                uploadItemDetail.ItemStatus = 1;
                uploadItemDetail.Reported = 0;
                uploadItemDetail.UploadDate = DateTime.Now;
                db.ItemDetail.Add(uploadItemDetail);
                db.SaveChanges();

                if (newForm.ImageUpload != null)
                {
                    Image uploadImage = imaging.AddImage(newForm.ImageUpload, uploadItem);
                }

                RedirectViewModel redirect = new RedirectViewModel("Details", "Items", uploadItem.ItemId.ToString());
                SaveNotification(uploadItem, redirect, NotificationType.AddItem);
                outputMessage = newForm.Name + " has been added to your items.";
                return true;
            }
            catch
            {
                outputMessage = Resources.Processing.ProcessError;
                return false;
            }
        }

        public bool EditUploadItem(UploadFormViewModel editForm, out string outputMessage)
        {
            string emailAddress = HttpContext.Current.User.Identity.Name.ToString();
            Account userAccount = global.GetAccount(emailAddress);
            try
            {
                Item referenceItem = db.Item.Where(i => i.ItemId == editForm.ReferenceId && i.AccountId == userAccount.AccountId).FirstOrDefault();
                if (referenceItem != null)
                {
                    referenceItem.Name = editForm.Name;
                    referenceItem.Caption = editForm.Caption;
                    referenceItem.Description = editForm.Description;
                    referenceItem.CategoryId = editForm.ItemCategory;
                    referenceItem.SubcategoryId = editForm.ItemSubcategory;
                    referenceItem.TradeCategoryId = editForm.TradeCategory;
                    referenceItem.TradeSubcategoryId = editForm.TradeSubcategory;
                    db.SaveChanges();

                    if (editForm.ImageKeep != 0)
                    {
                        if (editForm.ImageUpload != null)
                        {
                            imaging.DeleteImage(referenceItem.ItemId);
                            Image uploadImage = imaging.AddImage(editForm.ImageUpload, referenceItem);
                        }
                    }
                    else
                    {
                        imaging.DeleteImage(referenceItem.ItemId);
                    }

                    outputMessage = editForm.Name + " has been updated.";
                    RedirectViewModel redirect = new RedirectViewModel("Details", "Items", referenceItem.ItemId.ToString());
                    SaveNotification(referenceItem, redirect, NotificationType.EditItem);
                    return true;
                }
                else
                {
                    outputMessage = Resources.Processing.ProcessError;
                    return false;
                }
            }
            catch
            {
                outputMessage = Resources.Processing.ProcessError;
                return false;
            }
        }

        public void SaveNotification(dynamic reference, RedirectViewModel link, NotificationType type)
        {
            string emailAddress = HttpContext.Current.User.Identity.Name.ToString();
            Account userAccount = global.GetAccount(emailAddress);
            Item castedItem = reference;
            if (userAccount != null)
            {
                Notification newNote = new Notification();
                newNote.AccountId = userAccount.AccountId;
                switch (type)
                {
                    case NotificationType.AddItem:
                        newNote.Description = string.Format(Resources.Notifications.AddItem, castedItem.Name);
                        break;
                    case NotificationType.EditItem:
                        newNote.Description = string.Format(Resources.Notifications.EditItem, castedItem.Name);
                        break;
                }
                newNote.Action = link.Action;
                newNote.Contorller = link.Controller;
                newNote.Reference = link.Parameter;
                newNote.UploadDate = DateTime.Now;
                db.Notification.Add(newNote);
                db.SaveChanges();
            }
        }

        private bool uploadNewImage(HttpPostedFileBase file, out string fileUrl)
        {
            fileUrl = "";
            return true;
        }
    }
}