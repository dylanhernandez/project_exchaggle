using Exchaggle.Interfaces;
using Exchaggle.Models;
using Exchaggle.ViewModels.Common;
using Exchaggle.ViewModels.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Exchaggle.Enumerations;

namespace Exchaggle.Services
{
    public class ItemService : INotifiable
    {
        private ExchaggleDbContext db;
        private GlobalService global;
        private ImageService imaging;
        private TradeService tradeManager;

        public ItemService(ExchaggleDbContext databaseContext)
        {
            db = databaseContext;
            global = new GlobalService(db);
            imaging = new ImageService(db);
            tradeManager = new TradeService(db);
        }

        public IEnumerable<ItemBulletinViewModel> GetItemList()
        {
            string emailAddress = HttpContext.Current.User.Identity.Name.ToString();
            if (!global.IsUniqueEmailAddress(emailAddress))
            {
                Account userAccount = global.GetAccount(emailAddress);
                List<ItemBulletinViewModel> itemList = new List<ItemBulletinViewModel>();
                IEnumerable<Item> itemResults = db.Item.Where(i => i.AccountId == userAccount.AccountId).ToList();
                if (itemResults != null)
                {
                    ItemBulletinViewModel writeModel;
                    foreach (Item i in itemResults)
                    {
                        bool check = global.isConfirmedItem(i);
                        writeModel = new ItemBulletinViewModel();
                        writeModel.ItemId = i.ItemId;
                        writeModel.Name = i.Name;
                        writeModel.Caption = i.Caption;
                        writeModel.Description = i.Description;
                        Image itemImage = imaging.ServeImage(i);
                        if (itemImage != null)
                        {
                            writeModel.ImageSource = itemImage.ImageSource;
                        }
                        if (check)
                        {
                            writeModel.Status = 1;
                        }
                        itemList.Add(writeModel);
                    }
                }
                return itemList;
            }
            return null;
        }

        public IEnumerable<ItemBulletinViewModel> GetWishList()
        {
            string emailAddress = HttpContext.Current.User.Identity.Name.ToString();
            if (!global.IsUniqueEmailAddress(emailAddress))
            {
                Account userAccount = global.GetAccount(emailAddress);
                List<ItemBulletinViewModel> wishList = new List<ItemBulletinViewModel>();
                IEnumerable<Wishlist> itemResults = db.Wishlist.Where(i => i.AccountId == userAccount.AccountId).ToList();
                if (itemResults != null)
                {
                    ItemBulletinViewModel writeModel;
                    foreach (Wishlist w in itemResults)
                    {
                        Item findItem = db.Item.Where(i => i.ItemId == w.ItemId).FirstOrDefault();
                        if (findItem != null)
                        {
                            bool check = global.isConfirmedItem(findItem);
                            if (!check)
                            {
                                writeModel = new ItemBulletinViewModel();
                                writeModel.ItemId = findItem.ItemId;
                                writeModel.Name = findItem.Name;
                                writeModel.Caption = findItem.Caption;
                                writeModel.Description = findItem.Description;
                                Image itemImage = imaging.ServeImage(findItem);
                                if (itemImage != null)
                                {
                                    writeModel.ImageSource = itemImage.ImageSource;
                                }
                                wishList.Add(writeModel);
                            }
                        }
                    }
                }
                return wishList;
            }
            return null;
        }

        public bool DeleteItem(int itemId, out string outputMessage)
        {
            string emailAddress = HttpContext.Current.User.Identity.Name.ToString();
            if (!global.IsUniqueEmailAddress(emailAddress))
            {
                Account userAccount = global.GetAccount(emailAddress);
                try
                {
                    Item referenceItem = (userAccount != null) ? db.Item.Where(i => i.ItemId == itemId && i.AccountId == userAccount.AccountId).FirstOrDefault() : null;
                    if (referenceItem != null)
                    {
                        bool deleteReferenceTrades = tradeManager.DeleteOffersByItem(referenceItem);

                        List<Wishlist> referenceWishlists = db.Wishlist.Where(w => w.ItemId == referenceItem.ItemId).ToList();
                        if (referenceWishlists != null)
                        {
                            db.Wishlist.RemoveRange(referenceWishlists);
                            db.SaveChanges();
                        }

                        imaging.DeleteImage(referenceItem.ItemId);

                        RedirectViewModel redirect = new RedirectViewModel("Index", "Items", "");
                        SaveNotification(referenceItem, redirect, NotificationType.DeleteItem);
                        db.Item.Remove(referenceItem);
                        db.SaveChanges();
                        
                        outputMessage = "Your item has been deleted.";
                        return true;
                    }
                }
                catch
                {
                    outputMessage = Resources.Processing.ProcessError;
                }
            }
            outputMessage = Resources.Processing.ProcessError;
            return false;
        }

        public bool AppendWishlist(int itemId, out string outputMessage)
        {
            string emailAddress = HttpContext.Current.User.Identity.Name.ToString();
            if (!global.IsUniqueEmailAddress(emailAddress))
            {
                Account userAccount = global.GetAccount(emailAddress);
                try
                {
                    Item referenceItem = (userAccount != null) ? db.Item.Where(i => i.ItemId == itemId).FirstOrDefault() : null;
                    if (referenceItem != null)
                    {
                        Wishlist newItem = new Wishlist();
                        newItem.ItemId = referenceItem.ItemId;
                        newItem.AccountId = userAccount.AccountId;
                        db.Wishlist.Add(newItem);
                        db.SaveChanges();
                        RedirectViewModel redirect = new RedirectViewModel("Details", "Search", newItem.ItemId.ToString());
                        SaveNotification(referenceItem, redirect, NotificationType.AddWishList);
                        outputMessage = "You added " + referenceItem.Name + " to your wishlist.";
                        return true;
                    }
                }
                catch
                {
                    outputMessage = Resources.Processing.ProcessError;
                    return false;
                }
            }
            outputMessage = Resources.Processing.ProcessError;
            return false;
        }

        public bool DetachWishlist(int itemId, out string responseMessage)
        {
            string emailAddress = HttpContext.Current.User.Identity.Name.ToString();
            if (!global.IsUniqueEmailAddress(emailAddress))
            {
                Account userAccount = global.GetAccount(emailAddress);
                try
                {
                    Wishlist referenceItem = (userAccount != null) ? db.Wishlist.Where(w => w.ItemId == itemId && w.AccountId == userAccount.AccountId).FirstOrDefault() : null;
                    if (referenceItem != null)
                    {
                        Item checkItem = db.Item.Where(i => i.ItemId == referenceItem.ItemId).FirstOrDefault();
                        if (checkItem != null)
                        {
                            RedirectViewModel redirect = new RedirectViewModel("Wishlist", "Items", "");
                            SaveNotification(checkItem, redirect, NotificationType.RemoveWishlist);
                        }
                        db.Wishlist.Remove(referenceItem);
                        db.SaveChanges();
                        responseMessage = "Your wishlist item has been removed.";
                        return true;
                    }
                }
                catch
                {
                    responseMessage = Resources.Processing.ProcessError;
                    return false;
                }
            }
            responseMessage = Resources.Processing.ProcessError;
            return false;
        }

        public void SaveNotification(dynamic reference, RedirectViewModel link, NotificationType type)
        {
            string emailAddress = HttpContext.Current.User.Identity.Name.ToString();
            Account userAccount = global.GetAccount(emailAddress);

            if (userAccount != null)
            {
                Notification newNote = new Notification();
                newNote.AccountId = userAccount.AccountId;
                Item castedItem = reference;
                switch (type)
                {
                    case NotificationType.DeleteItem:
                        castedItem = reference;
                        newNote.Description = string.Format(Resources.Notifications.DeleteItem, castedItem.Name);
                        break;
                    case NotificationType.AddWishList:
                        newNote.Description = string.Format(Resources.Notifications.AddWishItem, castedItem.Name);
                        break;
                    case NotificationType.RemoveWishlist:
                        newNote.Description = string.Format(Resources.Notifications.RemoveWishItem, castedItem.Name);
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
    }
}