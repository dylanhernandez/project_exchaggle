using Exchaggle.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Exchaggle.ViewModels.Search
{
    public class SearchFormViewModel
    {
        private CountryStateViewModel countryState;

        public SearchFormViewModel()
        {
            countryState = new CountryStateViewModel();
        }

        public SearchFormViewModel(string defaultState = "AB")
        {
            countryState = new CountryStateViewModel(defaultState);
        }

        public SelectList CategoriesList { get; set; }
        public SelectList SubcategoriesList { get; set; }
        public SelectList CountryList { get { return countryState.CountryList; } }
        public SelectList StateList { get { return countryState.StateList; } }

        [Required(ErrorMessage = "Please enter something into the search box")]
        public string SearchQuery { get; set; }
        public int SearchCategory { get; set; }
        public int SearchSubcategory { get; set; }
        public int TradeCategory { get; set; }
        public int TradeSubcategory { get; set; }

        [Required(ErrorMessage = "State is required")]
        [Display(Name = "Province / State")]
        public string State { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }


    }
}