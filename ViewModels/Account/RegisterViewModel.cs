using Exchaggle.ViewModels.Account;
using Exchaggle.ViewModels.Common;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Exchaggle.ViewModels
{
    public class RegisterViewModel
    {
        private SecurityQuestionViewModel securityQuestions;
        private CountryStateViewModel countryState;

        public RegisterViewModel()
        {
            securityQuestions = new SecurityQuestionViewModel();
            countryState = new CountryStateViewModel();
        }

        public SelectList SecurityListA { get { return securityQuestions.GetSecurityQuestionListA; } }
        public SelectList SecurityListB { get { return securityQuestions.GetSecurityQuestionListB; } }
        public SelectList CountryList { get { return countryState.CountryList; } }
        public SelectList StateList { get { return countryState.StateList; } }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is invalid")]
        [Display(Name = "Email")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(int.MaxValue, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Password must be entered twice correctly")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Contact Name")]
        public string ContactName { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; }

        [Required(ErrorMessage = "State is required")]
        [Display(Name = "Province / State")]
        public string State { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [Phone]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Display(Name = "Security Question 1")]
        public string SecurityQuestionA { get; set; }

        [Display(Name = "Security Question 2")]
        public string SecurityQuestionB { get; set; }

        [Display(Name = "Answer")]
        [StringLength(50, ErrorMessage = "Security answer cannot exceed 50 characters")]
        public string SecurityAnswerA { get; set; }

        [Display(Name = "Answer")]
        [StringLength(50, ErrorMessage = "Security answer cannot exceed 50 characters")]
        public string SecurityAnswerB { get; set; }
    }
}
