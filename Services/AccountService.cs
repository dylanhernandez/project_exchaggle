using Exchaggle.Models;
using System.Web.Security;
using System.Linq;
using System.Web.Helpers;
using Exchaggle.ViewModels;
using Exchaggle.Enumerations;

namespace Exchaggle.Services
{
    public class AccountService
    {
        private ExchaggleDbContext db;
        private GlobalService global;
        private TradeService tradeChecker;

        public AccountService(ExchaggleDbContext databaseContext)
        {
            db = databaseContext;
            global = new GlobalService(db);
            tradeChecker = new TradeService(db);
        }

        public bool CreateAccount(RegisterViewModel newAccount, out string outputMessage)
        {
            if (!global.IsUniqueEmailAddress(newAccount.EmailAddress)) 
            {
                outputMessage = Resources.Processing.ProcessEmailExists;
                return false;
            }
            if (!global.IsUniqueUsername(newAccount.Username)) 
            {
                outputMessage = Resources.Processing.ProcessUsernameExists;
                return false;
            }

            Account registerAccount = new Account();
            registerAccount.EmailAddress = newAccount.EmailAddress.ToLower();
            registerAccount.Username = newAccount.Username;
            registerAccount.Password = Crypto.SHA1(newAccount.Password); ;
            registerAccount.ContactName = newAccount.ContactName;
            registerAccount.Country = (newAccount.Country != null) ? newAccount.Country : "Canada";
            registerAccount.State = (newAccount.State != null) ? newAccount.State : "ON";
            registerAccount.City = newAccount.City;
            registerAccount.Phone = newAccount.Phone;
            db.Account.Add(registerAccount);
            db.SaveChanges();

            AccountDetail registerDetails = new AccountDetail();
            registerDetails.AccountId = registerAccount.AccountId;
            registerDetails.AccountLevel = 1;
            registerDetails.AccountStatus = 1;
            registerDetails.SecurityQuestionA = newAccount.SecurityQuestionA;
            registerDetails.SecurityQuestionB = newAccount.SecurityQuestionB;
            registerDetails.SecurityAnswerA = newAccount.SecurityAnswerA;
            registerDetails.SecurityAnswerB = newAccount.SecurityAnswerB;
            db.AccountDetail.Add(registerDetails);
            db.SaveChanges();

            outputMessage = newAccount.Username + " is now registered";
            return true;
        }

        public bool AccessAccount(LoginViewModel loginAccount)
        {
            string emailAddress = loginAccount.EmailAddress.ToLower();
            Account userAccount = db.Account.Where(e => e.EmailAddress == emailAddress || e.Username.ToLower() == emailAddress).FirstOrDefault();
            if (userAccount != null)
            {
                AccountDetail userDetails = db.AccountDetail.Where(ad => ad.AccountId == userAccount.AccountId).FirstOrDefault();
                if (userDetails != null)
                {
                    if (userDetails.AccountStatus != (int)AccountStatusType.Cancelled)
                    {
                        var submittedPassword = Crypto.SHA1(loginAccount.Password);
                        if (submittedPassword == userAccount.Password)
                        {
                            tradeChecker.RunTradeExpirationCheck(userAccount);
                            FormsAuthentication.SetAuthCookie(userAccount.EmailAddress, true);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void EgressAccount()
        {
            FormsAuthentication.SignOut();
        }

        public PasswordRecoveryViewModel FindAccountSecurityDetails(PasswordRecoveryViewModel recoverAccount)
        {
            string emailAddress = recoverAccount.EmailAddress.ToLower();
            var checkAccount = db.Account.Where(c => c.EmailAddress == emailAddress).FirstOrDefault();
            if (checkAccount != null)
            {
                var accountDetails = db.AccountDetail.Where(ac => ac.AccountId == checkAccount.AccountId).FirstOrDefault();
                if (accountDetails != null)
                {
                    if (accountDetails.SecurityQuestionA == null || accountDetails.SecurityQuestionB == null)
                    {
                        return null;
                    }
                    recoverAccount.SecurityQuestionA = accountDetails.SecurityQuestionA;
                    recoverAccount.SecurityQuestionB = accountDetails.SecurityQuestionB;
                    return recoverAccount;
                }
            }
            return null;
        }

        public bool VerifyAccountSecurityDetails(PasswordRecoveryViewModel recoverAccount)
        {
            string emailAddress = recoverAccount.EmailAddress.ToLower();
            var checkAccount = db.Account.Where(c => c.EmailAddress == emailAddress).FirstOrDefault();
            if (checkAccount != null)
            {
                var accountDetails = db.AccountDetail.Where(ac => ac.AccountId == checkAccount.AccountId).FirstOrDefault();
                if (accountDetails != null)
                {
                    if (recoverAccount.SecurityAnswerA.ToLower() != accountDetails.SecurityAnswerA.ToLower() || recoverAccount.SecurityAnswerB.ToLower() != accountDetails.SecurityAnswerB.ToLower())
                    {
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }

        public bool ChangeAccountPassword(PasswordResetViewModel recoverAccount)
        {
            string emailAddress = recoverAccount.EmailAddress.ToLower();
            Account checkAccount = db.Account.Where(c => c.EmailAddress == emailAddress).FirstOrDefault();
            if (checkAccount != null)
            {
                if (recoverAccount.Password == recoverAccount.ConfirmPassword)
                {
                    checkAccount.Password = Crypto.SHA1(recoverAccount.Password);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}