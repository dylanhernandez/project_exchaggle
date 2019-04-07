using Exchaggle.Enumerations;
using Exchaggle.Models;
using Exchaggle.ViewModels;
using Exchaggle.ViewModels.Common;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace Exchaggle.Services
{
    public class ReportService
    {
        private ExchaggleDbContext db;
        private GlobalService global;
        private TradeService tradeManager;

        public ReportService(ExchaggleDbContext databaseContext)
        {
            db = databaseContext;
            global = new GlobalService(db);
            tradeManager = new TradeService(db);
        }

        public ReportViewModel GetReportSetup(int reference, ReportType type)
        {
            Item getItem = db.Item.Where(i => i.ItemId == reference).FirstOrDefault();
            if (getItem != null)
            {
                Account getAccount = db.Account.Where(a => a.AccountId == getItem.AccountId).FirstOrDefault();
                if (getAccount != null)
                {
                    ReportViewModel report = new ReportViewModel();
                    switch (type)
                    {
                        case ReportType.Item:
                            report.ReferenceId = getItem.ItemId;
                            report.Itemname = getItem.Name;
                            report.Username = getAccount.Username;
                            break;
                        case ReportType.Account:
                            report.ReferenceId = getAccount.AccountId;
                            report.Itemname = "";
                            report.Username = getAccount.Username;
                            break;
                        default:
                            return null;
                    }
                    return report;
                }
            }
            return null;
        }

        public bool SendReport(ReportViewModel reportForm, ReportType type, out string outputMessage)
        {
            string emailAddress = HttpContext.Current.User.Identity.Name.ToString();
            Account checkAccount = global.GetAccount(emailAddress);
            if (type == ReportType.Item)
            {
                Item getItem = db.Item.Where(i => i.ItemId == reportForm.ReferenceId).FirstOrDefault();
                if (getItem != null)
                {
                    Account getAccount = db.Account.Where(a => a.AccountId == getItem.AccountId).FirstOrDefault();
                    if (getAccount != null)
                    {
                        Report checkExisting = db.Report.Where(r => r.ReportableId == getItem.ItemId && r.ReportableType == (int)ReportType.Item && r.AccountId == checkAccount.AccountId).FirstOrDefault();
                        if (checkExisting == null)
                        {
                            ItemDetail getItemDetail = db.ItemDetail.Where(d => d.ItemId == getItem.ItemId).FirstOrDefault();
                            if (getItemDetail != null)
                            {
                                getItemDetail.Reported = 1;
                                db.SaveChanges();
                            }
                            try
                            {
                                Report newReport = new Report();
                                newReport.Description = reportForm.ReportDetails;
                                newReport.ReportableId = getItem.ItemId;
                                newReport.ReportableType = (int)ReportType.Item;
                                newReport.AccountId = checkAccount.AccountId;
                                db.Report.Add(newReport);
                                db.SaveChanges();
                                outputMessage = Resources.Processing.ProcessReportSent;
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
                            outputMessage = Resources.Processing.ProcessReportExists;
                            return false;
                        }
                    }
                }
            }
            else
            {
                Account getAccount = db.Account.Where(a => a.AccountId == reportForm.ReferenceId).FirstOrDefault();
                if (getAccount != null)
                {
                    Report checkExisting = db.Report.Where(r => r.ReportableId == getAccount.AccountId && r.ReportableType == (int)ReportType.Account && r.AccountId == checkAccount.AccountId).FirstOrDefault();
                    if (checkExisting == null)
                    {
                        AccountDetail getAccountDetail = db.AccountDetail.Where(d => d.AccountId == getAccount.AccountId).FirstOrDefault();
                        if (getAccountDetail != null)
                        {
                            getAccountDetail.AccountStatus = (int)AccountStatusType.Reported;
                            db.SaveChanges();
                        }
                        try
                        {
                            Report newReport = new Report();
                            newReport.Description = reportForm.ReportDetails;
                            newReport.ReportableId = getAccount.AccountId;
                            newReport.ReportableType = (int)ReportType.Account;
                            newReport.AccountId = checkAccount.AccountId;
                            db.Report.Add(newReport);
                            db.SaveChanges();
                            outputMessage = Resources.Processing.ProcessReportSent;
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
                        outputMessage = Resources.Processing.ProcessReportExists;
                        return false;
                    }
                }
            }
            outputMessage = Resources.Processing.ProcessError;
            return false;
        }

        public int ClearReportStatus(int reportId)
        {
            Report findReport = db.Report.Find(reportId);
            if (findReport != null)
            {
                int referenceType = findReport.ReportableType;
                if (referenceType == (int)ReportType.Item)
                {
                    ItemDetail reportableDetail = db.ItemDetail.Where(d => d.ItemId == findReport.ReportableId).FirstOrDefault();
                    if (reportableDetail != null)
                    {
                        db.Report.Remove(findReport);
                        reportableDetail.Reported = 0;
                        db.SaveChanges();
                        return referenceType;
                    }
                }
                else
                {
                    AccountDetail reportableDetail = db.AccountDetail.Where(d => d.AccountId == findReport.ReportableId).FirstOrDefault();
                    if (reportableDetail != null)
                    {
                        db.Report.Remove(findReport);
                        reportableDetail.AccountStatus = (int)AccountStatusType.Normal;
                        db.SaveChanges();
                        return referenceType;
                    }
                }
            }
            return -1;
        }

        public int RemoveReportable(int reportId)
        {
            Report findReport = db.Report.Find(reportId);
            if (findReport != null)
            {
                int referenceType = findReport.ReportableType;
                if (referenceType == (int)ReportType.Item)
                {
                    ItemDetail reportableDetail = db.ItemDetail.Where(d => d.ItemId == findReport.ReportableId && d.ItemStatus != (int)ItemStatusType.Confirmed).FirstOrDefault();
                    if (reportableDetail != null)
                    {
                        Item reportable = db.Item.Where(i => i.ItemId == reportableDetail.ItemId).FirstOrDefault();
                        if (reportable != null)
                        {
                            List<Wishlist> wishes = db.Wishlist.Where(w => w.ItemId == reportable.ItemId).ToList();
                            if (wishes != null)
                            {
                                db.Wishlist.RemoveRange(wishes).ToList();
                                db.SaveChanges();
                            }
                            List<Report> remainingReports = db.Report.Where(r => r.ReportableId == reportable.ItemId && r.ReportableType == (int)ReportType.Item && r.ReportId != findReport.ReportId).ToList();
                            if (remainingReports != null)
                            {
                                db.Report.RemoveRange(remainingReports).ToList();
                                db.SaveChanges();
                            }
                            tradeManager.DeleteOffersByItem(reportable);
                            db.Item.Remove(reportable);
                            db.SaveChanges();
                        }
                    }
                    db.Report.Remove(findReport);
                    db.SaveChanges();
                    return referenceType;
                }
                else
                {
                    Account reportableAccount = db.Account.Where(a => a.AccountId == findReport.ReportableId).FirstOrDefault();
                    if (reportableAccount != null)
                    {
                        AccountDetail userDetails = db.AccountDetail.Where(ad => ad.AccountId == reportableAccount.AccountId).FirstOrDefault();
                        if (userDetails != null)
                        {
                            List<Item> relatedItems = db.Item.Where(i => i.AccountId == reportableAccount.AccountId).ToList();
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
                                        List<Report> remainingReports = db.Report.Where(r => r.ReportableId == item.ItemId && r.ReportableType == (int)ReportType.Item && r.ReportId != findReport.ReportId).ToList();
                                        if (remainingReports != null)
                                        {
                                            db.Report.RemoveRange(remainingReports).ToList();
                                            db.SaveChanges();
                                        }
                                        tradeManager.DeleteOffersByItem(item);
                                        db.Item.Remove(item);
                                        db.SaveChanges();
                                    }
                                }
                            }
                            userDetails.AccountStatus = (int)AccountStatusType.Cancelled;
                            db.SaveChanges();
                        }
                    }
                    db.Report.Remove(findReport);
                    db.SaveChanges();
                    return referenceType;
                }
            }
            return -1;
        }
    }
}