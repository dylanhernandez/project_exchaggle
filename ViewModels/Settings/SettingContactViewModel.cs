using Exchaggle.ViewModels.Common;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Exchaggle.ViewModels.Settings
{
    public class SettingContactViewModel
    {
        private CountryStateViewModel countryState;

        public SettingContactViewModel()
        {
            countryState = new CountryStateViewModel();
        }

        public SettingContactViewModel(string defState = "AB")
        {
            countryState = new CountryStateViewModel(defState);
        }

        public SelectList Countries { get { return countryState.CountryList; } }
        public SelectList States { get { return countryState.StateList; } }

        [Display(Name = "Contact Name")]
        public string ContactName { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; }

        [Required(ErrorMessage = "State is required")]
        [Display(Name = "Province / State")]
        public string State { get; set; }

        public string City { get; set; }

        [Phone(ErrorMessage = "Phone number is in the incorrect format")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
    }
}