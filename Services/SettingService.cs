using Exchaggle.Enumerations;
using Exchaggle.Models;
using Exchaggle.ViewModels;
using Exchaggle.ViewModels.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace Exchaggle.Services
{
    public class SettingService
    {
        private ExchaggleDbContext db;
        private GlobalService global;
        private TradeService tradeManager;

        public SettingService(ExchaggleDbContext databaseContext)
        {
            db = databaseContext;
            global = new GlobalService(db);
            tradeManager = new TradeService(db);
        }

        public bool ChangeUsername(SettingUsernameViewModel settingForm, out string outputMessage)
        {
            string emailAddress = HttpContext.Current.User.Identity.Name.ToString();
            Account userAccount = global.GetAccount(emailAddress);
            if (userAccount != null)
            {
                if (userAccount.Username.ToLower() == settingForm.OldUsername.ToLower())
                {
                    if (global.IsUniqueUsername(settingForm.NewUsername))
                    {
                        try
                        {
                            userAccount.Username = settingForm.NewUsername;
                            db.SaveChanges();
                            outputMessage = string.Format(Resources.Processing.ProcessSettingsConfirmed, "username");
                            return true;
                        }
                        catch
                        {
                            outputMessage = Resources.Processing.ProcessError;
                            return false;
                        }
                    }
                    else
                    {
                        outputMessage = Resources.Processing.ProcessUsernameExists;
                        return false;
                    }
                }
                else
                {
                    outputMessage = Resources.Processing.ProcessUsernameNotFound;
                    return false;
                }
            }
            outputMessage = Resources.Processing.ProcessError;
            return false;
        }

        public bool ChangeEmail(SettingEmailViewModel settingForm, out string outputMessage)
        {
            string emailAddress = HttpContext.Current.User.Identity.Name.ToString();
            Account userAccount = global.GetAccount(emailAddress);
            if (userAccount != null)
            {
                if (userAccount.EmailAddress.ToLower() == settingForm.OldEmail.ToLower())
                {
                    if (global.IsUniqueEmailAddress(settingForm.NewEmail))
                    {
                        try
                        {
                            userAccount.EmailAddress = settingForm.NewEmail;
                            db.SaveChanges();
                            outputMessage = string.Format(Resources.Processing.ProcessSettingsConfirmed, "email address");
                            return true;
                        }
                        catch
                        {
                            outputMessage = Resources.Processing.ProcessError;
                            return false;
                        }
                    }
                    else
                    {
                        outputMessage = Resources.Processing.ProcessEmailExists;
                        return false;
                    }
                }
                else
                {
                    outputMessage = Resources.Processing.ProcessEmailNotFound;
                    return false;
                }
            }
            outputMessage = Resources.Processing.ProcessError;
            return false;
        }

        public bool ChangePassword(SettingPasswordViewModel settingForm, out string outputMessage)
        {
            string emailAddress = HttpContext.Current.User.Identity.Name.ToString();
            Account userAccount = global.GetAccount(emailAddress);
            if (userAccount != null)
            {
                string oldPassEncrypt = Crypto.SHA1(settingForm.OldPassword);
                string newPassEncrypt = Crypto.SHA1(settingForm.NewPassword);
                string newPassEncryptConfirm = Crypto.SHA1(settingForm.ConfirmNewPassword);
                if (userAccount.Password == oldPassEncrypt)
                {
                    if (newPassEncrypt == newPassEncryptConfirm)
                    {
                        try
                        {
                            userAccount.Password = newPassEncrypt;
                            db.SaveChanges();
                            outputMessage = string.Format(Resources.Processing.ProcessSettingsConfirmed, "password");
                            return true;
                        }
                        catch
                        {
                            outputMessage = Resources.Processing.ProcessError;
                            return false;
                        }
                    }
                    else
                    {
                        outputMessage = Resources.Processing.ProcessPasswordNoMatch;
                        return false;
                    }
                }
                else
                {
                    outputMessage = Resources.Processing.ProcessPasswordIncorrect;
                    return false;
                }
            }
            outputMessage = Resources.Processing.ProcessError;
            return false;
        }

        public bool ChangeContactInformation(SettingContactViewModel settingForm, out string outputMessage)
        {
            string emailAddress = HttpContext.Current.User.Identity.Name.ToString();
            Account userAccount = global.GetAccount(emailAddress);
            if (userAccount != null)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(settingForm.ContactName))
                    {
                        userAccount.ContactName = settingForm.ContactName;
                        db.SaveChanges();
                    }
                    if (!string.IsNullOrWhiteSpace(settingForm.Country))
                    {
                        userAccount.Country = settingForm.Country;
                        db.SaveChanges();
                    }
                    if (!string.IsNullOrWhiteSpace(settingForm.Phone))
                    {
                        userAccount.Phone = settingForm.Phone;
                        db.SaveChanges();
                    }
                    if (!string.IsNullOrWhiteSpace(settingForm.State))
                    {
                        userAccount.State = settingForm.State;
                        db.SaveChanges();
                    }
                    if (!string.IsNullOrWhiteSpace(settingForm.City))
                    {
                        userAccount.City = settingForm.City;
                        db.SaveChanges();
                    }
                    outputMessage = string.Format(Resources.Processing.ProcessSettingsConfirmed, "contact information");
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

        public bool CancelAccount(LoginViewModel settingForm, out string outputMessage)
        {
            string emailAddress = HttpContext.Current.User.Identity.Name.ToString();
            Account userAccount = global.GetAccount(emailAddress);
            if (userAccount != null)
            {
                string emailUsername = settingForm.EmailAddress.ToLower();
                string setPassword = Crypto.SHA1(settingForm.Password);
                if ((userAccount.EmailAddress.ToLower() == emailUsername || userAccount.Username.ToLower() == emailUsername) && userAccount.Password == setPassword)
                {
                    AccountDetail userDetails = db.AccountDetail.Where(ad => ad.AccountId == userAccount.AccountId).FirstOrDefault();
                    if (userDetails != null)
                    {
                        List<Item> relatedItems = db.Item.Where(i => i.AccountId == userAccount.AccountId).ToList();
                        if (relatedItems != null)
                        {
                            foreach (Item item in relatedItems)
                            {
                                ItemDetail detail = db.ItemDetail.Where(d => d.ItemId == item.ItemId && d.ItemStatus != (int)ItemStatusType.Confirmed).FirstOrDefault();
                                if (detail != null)
                                {
                                    List<Wishlist> wishes = db.Wishlist.Where(w => w.ItemId == item.ItemId).ToList();
                                    if (wishes != null)
                                    {
                                        db.Wishlist.RemoveRange(wishes).ToList();
                                        db.SaveChanges();
                                    }
                                    tradeManager.DeleteOffersByItem(item);
                                    db.Item.Remove(item);
                                }
                            }
                            db.SaveChanges();
                        }
                        List<Notification> notes = db.Notification.Where(n => n.AccountId == userAccount.AccountId).ToList();
                        if (notes != null)
                        {
                            db.Notification.RemoveRange(notes);
                            db.SaveChanges();
                        }
                        userDetails.AccountStatus = (int)AccountStatusType.Cancelled;
                        db.SaveChanges();
                        outputMessage = Resources.Processing.ProcessAccountRemoved;
                        return true;
                    }
                }
                else
                {
                    outputMessage = Resources.Processing.ProcessIncorrectLogin;
                    return false;
                }
            }
            outputMessage = Resources.Processing.ProcessError;
            return false;
        }
    }
}