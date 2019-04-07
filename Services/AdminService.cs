using Exchaggle.Enumerations;
using Exchaggle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Exchaggle.Services
{
    public class AdminService
    {
        private ExchaggleDbContext db;
        private GlobalService global;

        public AdminService(ExchaggleDbContext databaseContext)
        {
            db = databaseContext;
            global = new GlobalService(db);
        }

        public bool CheckAccountLevel()
        {
            string emailAddress = HttpContext.Current.User.Identity.Name.ToString();
            Account userAccount = global.GetAccount(emailAddress);
            if (userAccount != null)
            {
                AccountDetail userDetail = db.AccountDetail.Where(ad => ad.AccountId == userAccount.AccountId).FirstOrDefault();
                if (userDetail != null)
                {
                    if (userDetail.AccountLevel == (int)AccountLevelType.Admin)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}