using Exchaggle.Interfaces;
using Exchaggle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Exchaggle.Enumerations;
using Exchaggle.ViewModels.Common;
using Exchaggle.ViewModels.Trades;
using Exchaggle.ViewModels.Items;

namespace Exchaggle.Services
{
    public class TradeService : INotifiable
    {
        private ExchaggleDbContext db;
        private GlobalService global;
        private ImageService imaging;

        public TradeService(ExchaggleDbContext databaseContext)
        {
            db = databaseContext;
            global = new GlobalService(db);
            imaging = new ImageService(db);
        }

        public TradesDetailViewModel GetTradeDetails(int receiver, int sender)
        {
            Offer offerRequest = db.Offer.Where(o => o.ReceiverItemId == receiver && o.SenderItemId == sender).FirstOrDefault();
            if (offerRequest != null)
            {
                OfferDetail offerRequestDetails = db.OfferDetail.Where(od => od.OfferId == offerRequest.OfferId).FirstOrDefault();
                ItemsDetailViewModel receiverItem = global.GetItemDetail(receiver);
                ItemsDetailViewModel senderItem = global.GetItemDetail(sender);
                if (receiverItem != null && senderItem != null && offerRequestDetails != null)
                {
                    TradesDetailViewModel detailOffer = new TradesDetailViewModel();
                    detailOffer.OfferId = offerRequest.OfferId;
                    detailOffer.Confirmed = offerRequestDetails.Confirmed;
                    detailOffer.ExpirationDate = offerRequestDetails.ExpirationDate;
                    detailOffer.ReceiverItem = receiverItem;
                    detailOffer.SenderItem = senderItem;
                    if (offerRequestDetails.Confirmed == 1)
                    {
                        Account receiverAccount = db.Account.Where(a => a.AccountId == offerRequest.ReceiverId).FirstOrDefault();
                        Account senderAccount = db.Account.Where(a => a.AccountId == offerRequest.AccountId).FirstOrDefault();
                        detailOffer.ReceiverDetails = getContactInformation(receiverAccount);
                        detailOffer.SenderDetails = getContactInformation(senderAccount);
                    }
                    return detailOffer;
                }
            }
            return null;
        }

        public MakeOfferViewModel CreateOfferList(int reference)
        {
            Item referenceItem = db.Item.Where(i => i.ItemId == reference).FirstOrDefault();
            if (referenceItem != null)
            {
                MakeOfferViewModel offerList = new MakeOfferViewModel();
                offerList.ReferenceId = referenceItem.ItemId;
                offerList.Name = referenceItem.Name;
                offerList.Caption = referenceItem.Caption;
                offerList.Description = referenceItem.Description;
                offerList.OfferOptions = GetItemList(reference);
                return offerList;
            }
            return null;
        }

        public OfferBulletinViewModel CreateOfferPrompt(int referenceA, int referenceB)
        {
            bool check = isOffered(referenceA, referenceB);
            if (!check)
            {
                Item receiverItem = db.Item.Where(i => i.ItemId == referenceA).FirstOrDefault();
                Account receiverAccount = db.Account.Where(ra => ra.AccountId == receiverItem.AccountId).FirstOrDefault();
                Item senderItem = db.Item.Where(i => i.ItemId == referenceB).FirstOrDefault();
                Account senderAccount = db.Account.Where(ra => ra.AccountId == senderItem.AccountId).FirstOrDefault();
                if (receiverItem != null && senderItem != null)
                {
                    OfferBulletinViewModel offerMake = new OfferBulletinViewModel();
                    offerMake.Confirmed = 0;
                    offerMake.ItemIdA = receiverItem.ItemId;
                    offerMake.NameA = receiverItem.Name;
                    offerMake.CaptionA = receiverItem.Caption;
                    offerMake.DescriptionA = receiverItem.Description;
                    offerMake.UserA = receiverAccount.Username;
                    offerMake.ItemIdB = senderItem.ItemId;
                    offerMake.NameB = senderItem.Name;
                    offerMake.CaptionB = senderItem.Caption;
                    offerMake.DescriptionB = senderItem.Description;
                    offerMake.UserB = senderAccount.Username;
                    return offerMake;
                }
            }
            return null;
        }

        public bool CreateOffer(int referenceA, int referenceB)
        {
            string emailAddress = HttpContext.Current.User.Identity.Name.ToString();
            if (!global.IsUniqueEmailAddress(emailAddress))
            {
                Account userAccount = global.GetAccount(emailAddress);
                try
                {
                    Item receiverItem = db.Item.Where(i => i.ItemId == referenceA).FirstOrDefault();
                    Item senderItem = db.Item.Where(i => i.ItemId == referenceB && i.AccountId == userAccount.AccountId).FirstOrDefault();
                    if (receiverItem != null && senderItem != null)
                    {
                        Offer newOffer = new Offer();
                        newOffer.AccountId = userAccount.AccountId;
                        newOffer.SenderItemId = senderItem.ItemId;
                        newOffer.ReceiverId = receiverItem.AccountId;
                        newOffer.ReceiverItemId = receiverItem.ItemId;
                        db.Offer.Add(newOffer);
                        db.SaveChanges();

                        OfferDetail newDetail = new OfferDetail();
                        newDetail.OfferId = newOffer.OfferId;
                        newDetail.Confirmed = 0;
                        newDetail.UploadDate = DateTime.Now;
                        newDetail.ExpirationDate = DateTime.Now.AddDays(14);
                        db.OfferDetail.Add(newDetail);
                        db.SaveChanges();

                        RedirectViewModel redirectSender = new RedirectViewModel("Index", "Trades", "");
                        RedirectViewModel redirectReceiver = new RedirectViewModel("UserOffers", "Trades", "");
                        SaveNotification(newOffer, redirectSender, NotificationType.AddTradeSender);
                        SaveNotification(newOffer, redirectReceiver, NotificationType.AddTradeReceiver);
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

        public bool DeleteOffer(int referenceA, int referenceB, out string outputMessage)
        {
            Offer referenceOffer = db.Offer.Where(o => o.ReceiverItemId == referenceA && o.SenderItemId == referenceB).FirstOrDefault();
            if (referenceOffer != null)
            {
                OfferDetail referenceDetail = db.OfferDetail.Where(od => od.OfferId == referenceOffer.OfferId).FirstOrDefault();
                try
                {
                    if (referenceDetail != null)
                    {
                        db.OfferDetail.Remove(referenceDetail);
                    }
                    RedirectViewModel redirectSender = new RedirectViewModel("Index", "Trades", "");
                    RedirectViewModel redirectReceiver = new RedirectViewModel("UserOffers", "Trades", "");
                    SaveNotification(referenceOffer, redirectSender, NotificationType.DeleteTradeSender);
                    SaveNotification(referenceOffer, redirectReceiver, NotificationType.DeleteTradeReceiver);
                    db.Offer.Remove(referenceOffer);
                    db.SaveChanges();
                    outputMessage = "Offer has been removed.";
                    return true;
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

        public bool DeleteOffersByItem(Item referenceItem)
        {
            if (referenceItem != null)
            {
                List<Offer> referenceOffers = db.Offer.Where(o => o.ReceiverItemId == referenceItem.ItemId || o.SenderItemId == referenceItem.ItemId).ToList();
                if (referenceOffers != null)
                {
                    foreach (Offer offer in referenceOffers)
                    {
                        OfferDetail referenceDetail = db.OfferDetail.Where(od => od.OfferId == offer.OfferId).FirstOrDefault();
                        if (referenceDetail != null)
                        {
                            db.OfferDetail.Remove(referenceDetail);
                        }
                        RedirectViewModel redirectSender = new RedirectViewModel("Index", "Trades", "");
                        RedirectViewModel redirectReceiver = new RedirectViewModel("UserOffers", "Trades", "");
                        SaveNotification(offer, redirectSender, NotificationType.DeleteTradeSender);
                        SaveNotification(offer, redirectReceiver, NotificationType.DeleteTradeReceiver);
                        db.Offer.Remove(offer);
                        db.SaveChanges();
                    }
                }
                return true;
            }
            return false;
        }

        public bool ConfirmOffer(int referenceA, int referenceB, out string outputMessage)
        {
            Offer referenceOffer = db.Offer.Where(o => o.ReceiverItemId == referenceA && o.SenderItemId == referenceB).FirstOrDefault();
            if (referenceOffer != null)
            {
                OfferDetail referenceDetail = db.OfferDetail.Where(od => od.OfferId == referenceOffer.OfferId).FirstOrDefault();
                if (referenceDetail != null)
                {
                    try
                    {
                        ItemDetail receiverDetail = db.ItemDetail.Where(id => id.ItemId == referenceOffer.ReceiverItemId).FirstOrDefault();
                        ItemDetail senderDetail = db.ItemDetail.Where(id => id.ItemId == referenceOffer.SenderItemId).FirstOrDefault();
                        if (receiverDetail != null && senderDetail != null)
                        {
                            receiverDetail.ItemStatus = (int)ItemStatusType.Confirmed;
                            senderDetail.ItemStatus = (int)ItemStatusType.Confirmed;
                            db.SaveChanges();
                        }
                        referenceDetail.ExpirationDate = DateTime.Now.AddDays(14);
                        referenceDetail.Confirmed = 1;
                        db.SaveChanges();

                        List<Offer> referenceOffers = db.Offer.Where(o => o.ReceiverItemId == referenceA).ToList();
                        if (referenceOffers != null)
                        {
                            foreach (Offer offer in referenceOffers)
                            {
                                OfferDetail offerDetail = db.OfferDetail.Where(od => od.OfferId == offer.OfferId && od.Confirmed == 0).FirstOrDefault();
                                if (offerDetail != null)
                                {
                                    db.OfferDetail.Remove(offerDetail);
                                    RedirectViewModel redirectSenderReject = new RedirectViewModel("Index", "Trades", "");
                                    SaveNotification(offer, redirectSenderReject, NotificationType.DeleteTradeSender);
                                    db.Offer.Remove(offer);
                                    db.SaveChanges();
                                }
                            }
                        }

                        List<Wishlist> referenceWishlists = db.Wishlist.Where(w => w.ItemId == referenceOffer.ReceiverItemId || w.ItemId == referenceOffer.SenderItemId).ToList();
                        if (referenceWishlists != null)
                        {
                            db.Wishlist.RemoveRange(referenceWishlists);
                            db.SaveChanges();
                        }

                        RedirectViewModel redirectSender = new RedirectViewModel("Index", "Trades", "");
                        RedirectViewModel redirectReceiver = new RedirectViewModel("UserOffers", "Trades", "");
                        SaveNotification(referenceOffer, redirectSender, NotificationType.ConfirmTradeSender);
                        SaveNotification(referenceOffer, redirectReceiver, NotificationType.ConfirmTradeReceiver);
                    }
                    catch
                    {
                        outputMessage = Resources.Processing.ProcessError;
                        return false;
                    }
                    outputMessage = "Offer has been confirmed.";
                    return true;
                }
            }
            outputMessage = Resources.Processing.ProcessError;
            return false;
        }

        public void RunTradeExpirationCheck(Account referenceAccount)
        {
            if (referenceAccount != null)
            {
                var offerResults = db.Offer.Join(db.OfferDetail, o => o.OfferId, od => od.OfferId, (o, od) => new { o, od })
                    .OrderByDescending(ord => ord.od.UploadDate)
                    .Where(ofr => (ofr.o.AccountId == referenceAccount.AccountId || ofr.o.ReceiverId == referenceAccount.AccountId) && ofr.od.Confirmed == 1).ToList();
                if (offerResults != null)
                {
                    if (offerResults.Count > 0)
                    {
                        DateTime currentTime = DateTime.Today;
                        foreach (var offer in offerResults)
                        {
                            DateTime recordedDate = offer.od.ExpirationDate;
                            if (recordedDate <= currentTime)
                            {
                                RedirectViewModel redirectSenderExpire = new RedirectViewModel("Index", "Trades", "");
                                RedirectViewModel redirectReceiverExpire = new RedirectViewModel("UserOffers", "Trades", "");
                                SaveNotification(offer.o, redirectSenderExpire, NotificationType.TradeExpiredSender);
                                SaveNotification(offer.o, redirectReceiverExpire, NotificationType.TradeExpiredReceiver);
                                Offer expiredOffer = db.Offer.Where(off => off.OfferId == offer.o.OfferId).FirstOrDefault();
                                if (expiredOffer != null)
                                {
                                    Item receiverItem = db.Item.Where(i => i.ItemId == expiredOffer.ReceiverItemId).FirstOrDefault();
                                    Item senderItem = db.Item.Where(i => i.ItemId == expiredOffer.SenderItemId).FirstOrDefault();
                                    if (receiverItem != null)
                                    {
                                        db.Item.Remove(receiverItem);
                                    }
                                    if (senderItem != null)
                                    {
                                        db.Item.Remove(senderItem);
                                    }
                                    db.Offer.Remove(expiredOffer);
                                    db.SaveChanges();
                                }
                            }
                            else
                            {
                                double difference = Math.Round((recordedDate - currentTime).TotalDays);
                                if (difference <= 3)
                                {
                                    if (offer.o.AccountId == referenceAccount.AccountId)
                                    {
                                        RedirectViewModel redirectSenderExpireSoon = new RedirectViewModel("Index", "Trades", "");
                                        SaveNotification(offer.o, redirectSenderExpireSoon, NotificationType.TradeExpireWarningSender);
                                    }
                                    else
                                    {
                                        RedirectViewModel redirectReceiverExpireSoon = new RedirectViewModel("UserOffers", "Trades", "");
                                        SaveNotification(offer.o, redirectReceiverExpireSoon, NotificationType.TradeExpireWarningReceiver);
                                    }
                                }
                            }
                        }
                    }
                }   
            }
        }

        public IEnumerable<OfferBulletinViewModel> OfferList(OfferType type, int reference = -1)
        {
            string emailAddress = HttpContext.Current.User.Identity.Name.ToString();
            if (!global.IsUniqueEmailAddress(emailAddress))
            {
                Account userAccount = global.GetAccount(emailAddress);
                List<OfferBulletinViewModel> offerList = new List<OfferBulletinViewModel>();
                dynamic offerResults = null;
                switch (type)
                {
                    case OfferType.Receiver:
                        offerResults = (reference == -1) ?
                            db.Offer.Join(db.OfferDetail, o => o.OfferId, od => od.OfferId, (o, od) => new { o, od })
                            .OrderByDescending(ord => ord.od.UploadDate)
                            .Where(ofr => ofr.o.ReceiverId == userAccount.AccountId).ToList() :
                            db.Offer.Join(db.OfferDetail, o => o.OfferId, od => od.OfferId, (o, od) => new { o, od })
                            .OrderByDescending(ord => ord.o.ReceiverItemId == reference).ThenBy(ord => ord.od.UploadDate)
                            .Where(ofr => ofr.o.ReceiverId == userAccount.AccountId).ToList();
                        break;
                    case OfferType.Sender:
                        offerResults = db.Offer.Join(db.OfferDetail, o => o.OfferId, od => od.OfferId, (o, od) => new { o, od })
                            .OrderByDescending(ord => ord.od.UploadDate)
                            .Where(ofr => ofr.o.AccountId == userAccount.AccountId).ToList();
                        break;
                }
                if (offerResults != null)
                {
                    if (offerResults.Count > 0)
                    {
                        foreach (var offer in offerResults)
                        {
                            int recId = offer.o.ReceiverItemId;
                            int senId = offer.o.SenderItemId;
                            Item receiverItem = db.Item.Where(i => i.ItemId == recId).FirstOrDefault();
                            Account receiverAccount = db.Account.Where(ra => ra.AccountId == receiverItem.AccountId).FirstOrDefault();
                            Item senderItem = db.Item.Where(i => i.ItemId == senId).FirstOrDefault();
                            Account senderAccount = db.Account.Where(ra => ra.AccountId == senderItem.AccountId).FirstOrDefault();
                            if (receiverItem != null && senderItem != null)
                            {
                                OfferBulletinViewModel offerMake = new OfferBulletinViewModel();
                                offerMake.Confirmed = offer.od.Confirmed;
                                offerMake.ItemIdA = receiverItem.ItemId;
                                offerMake.NameA = receiverItem.Name;
                                offerMake.CaptionA = receiverItem.Caption;
                                offerMake.DescriptionA = receiverItem.Description;
                                offerMake.UserA = receiverAccount.Username;
                                Image imageA = imaging.ServeImage(receiverItem.ItemId);
                                if (imageA != null)
                                {
                                    offerMake.ImageStringA = imageA.ImageSource;
                                }
                                offerMake.ItemIdB = senderItem.ItemId;
                                offerMake.NameB = senderItem.Name;
                                offerMake.CaptionB = senderItem.Caption;
                                offerMake.DescriptionB = senderItem.Description;
                                offerMake.UserB = senderAccount.Username;
                                Image imageB = imaging.ServeImage(senderItem.ItemId);
                                if (imageB != null)
                                {
                                    offerMake.ImageStringB = imageB.ImageSource;
                                }
                                offerList.Add(offerMake);
                            }

                        }
                    }
                    return offerList;
                }
            }
            return null;
        }

        public IEnumerable<ItemBulletinViewModel> GetItemList(int check)
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
                        bool confirmed = global.isConfirmedItem(i);
                        bool offered = isOffered(check, i.ItemId);
                        if (!confirmed && !offered)
                        {
                            writeModel = new ItemBulletinViewModel();
                            writeModel.ItemId = i.ItemId;
                            writeModel.Name = i.Name;
                            writeModel.Caption = i.Caption;
                            writeModel.Description = i.Description;
                            itemList.Add(writeModel);
                        }
                    }
                }
                return itemList;
            }
            return null;
        }

        public void SaveNotification(dynamic reference, RedirectViewModel link, NotificationType type)
        {
            Offer castOffer = reference;
            Item receiverItem = db.Item.Where(i => i.ItemId == castOffer.ReceiverItemId).FirstOrDefault();
            Item senderItem = db.Item.Where(i => i.ItemId == castOffer.SenderItemId).FirstOrDefault();
            if (receiverItem != null && senderItem != null)
            {
                Notification newNote = new Notification();
                switch (type)
                {
                    case NotificationType.DeleteTradeReceiver:
                        newNote.AccountId = receiverItem.AccountId;
                        newNote.Description = string.Format(Resources.Notifications.RemoveTrade, senderItem.Name, receiverItem.Name);
                        break;
                    case NotificationType.DeleteTradeSender:
                        newNote.AccountId = senderItem.AccountId;
                        newNote.Description = string.Format(Resources.Notifications.RemoveTrade, senderItem.Name, receiverItem.Name);
                        break;
                    case NotificationType.AddTradeReceiver:
                        newNote.AccountId = receiverItem.AccountId;
                        newNote.Description = string.Format(Resources.Notifications.AddTradeReceiver, receiverItem.Name, senderItem.Name);
                        break;
                    case NotificationType.AddTradeSender:
                        newNote.AccountId = senderItem.AccountId;
                        newNote.Description = string.Format(Resources.Notifications.AddTradeSender, senderItem.Name, receiverItem.Name);
                        break;
                    case NotificationType.ConfirmTradeReceiver:
                        newNote.AccountId = receiverItem.AccountId;
                        newNote.Description = string.Format(Resources.Notifications.ConfirmTrade, senderItem.Name, receiverItem.Name);
                        break;
                    case NotificationType.ConfirmTradeSender:
                        newNote.AccountId = senderItem.AccountId;
                        newNote.Description = string.Format(Resources.Notifications.ConfirmTrade, senderItem.Name, receiverItem.Name);
                        break;
                    case NotificationType.TradeExpireWarningReceiver:
                        newNote.AccountId = receiverItem.AccountId;
                        newNote.Description = string.Format(Resources.Notifications.ExpireWarning, senderItem.Name, receiverItem.Name);
                        break;
                    case NotificationType.TradeExpiredReceiver:
                        newNote.AccountId = receiverItem.AccountId;
                        newNote.Description = string.Format(Resources.Notifications.Expired, senderItem.Name, receiverItem.Name);
                        break;
                    case NotificationType.TradeExpireWarningSender:
                        newNote.AccountId = senderItem.AccountId;
                        newNote.Description = string.Format(Resources.Notifications.ExpireWarning, senderItem.Name, receiverItem.Name);
                        break;
                    case NotificationType.TradeExpiredSender:
                        newNote.AccountId = senderItem.AccountId;
                        newNote.Description = string.Format(Resources.Notifications.Expired, senderItem.Name, receiverItem.Name);
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

        private ContactDetailViewModel getContactInformation(Account reference)
        {
            if (reference != null)
            {
                ContactDetailViewModel contact = new ContactDetailViewModel();
                contact.Email = reference.EmailAddress;
                contact.City = reference.City;
                contact.ContactName = reference.ContactName;
                contact.Country = reference.Country;
                contact.Phone = reference.Phone;
                contact.State = reference.State;
                return contact;
            }
            return null;
        }

        private bool isOffered(int itemIdA, int itemIdB)
        {
            Offer checkOffer = db.Offer.Where(o => (o.ReceiverItemId == itemIdA && o.SenderItemId == itemIdB) || (o.ReceiverItemId == itemIdB && o.SenderItemId == itemIdA)).FirstOrDefault();
            if (checkOffer != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}