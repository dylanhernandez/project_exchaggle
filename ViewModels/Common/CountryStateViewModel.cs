using System.Collections.Generic;
using System.Web.Mvc;

namespace Exchaggle.ViewModels.Common
{
    public class CountryStateViewModel
    {
        private string defaultStateValue;

        public CountryStateViewModel()
        {
            defaultStateValue = "ON";
        }

        public CountryStateViewModel(string state = "ON")
        {
            defaultStateValue = state;
        }

        public SelectList CountryList
        {
            get
            {
                return new SelectList(new List<object>
                {
                    new { value = "Canada" , text = "Canada"  },
                    new { value = "United States" ,  text = "United States" }
                }, "value", "text", "Canada");
            }
        }

        public SelectList StateList
        {
            get
            {
                return new SelectList(new List<object>
                {
                    new { value = "AK" , text = "AK - Alaska"},
                    new { value = "AL" , text = "AL - Alabama"},
                    new { value = "AR" , text = "AR - Arkansas"},
                    new { value = "AZ" , text = "AZ - Arizona"},
                    new { value = "CA" , text = "CA - California"},
                    new { value = "CO" , text = "CO - Colorado"},
                    new { value = "CT" , text = "CT - Connecticut"},
                    new { value = "DC" , text = "DC - District of Columbia"},
                    new { value = "DE" , text = "DE - Delaware"},
                    new { value = "FL" , text = "FL - Florida"},
                    new { value = "GA" , text = "GA - Georgia"},
                    new { value = "GU" , text = "GU - Guam"},
                    new { value = "HI" , text = "HI - Hawaii"},
                    new { value = "IA" , text = "IA - Iowa"},
                    new { value = "ID" , text = "ID - Idaho"},
                    new { value = "IL" , text = "IL - Illinois"},
                    new { value = "IN" , text = "IN - Indiana"},
                    new { value = "KS" , text = "KS - Kansas"},
                    new { value = "KY" , text = "KY - Kentucky"},
                    new { value = "LA" , text = "LA - Louisiana"},
                    new { value = "MA" , text = "MA - Massachusetts"},
                    new { value = "MD" , text = "MD - Maryland"},
                    new { value = "ME" , text = "ME - Maine"},
                    new { value = "MI" , text = "MI - Michigan"},
                    new { value = "MN" , text = "MN - Minnesota"},
                    new { value = "MO" , text = "MO - Missouri"},
                    new { value = "MS" , text = "MS - Mississippi"},
                    new { value = "MT" , text = "MT - Montana"},
                    new { value = "NC" , text = "NC - North Carolina"},
                    new { value = "ND" , text = "ND - North Dakota"},
                    new { value = "NE" , text = "NE - Nebraska"},
                    new { value = "NH" , text = "NH - New Hampshire"},
                    new { value = "NJ" , text = "NJ - New Jersey"},
                    new { value = "NM" , text = "NM - New Mexico"},
                    new { value = "NV" , text = "NV - Nevada"},
                    new { value = "NY" , text = "NY - New York"},
                    new { value = "OH" , text = "OH - Ohio"},
                    new { value = "OK" , text = "OK - Oklahoma"},
                    new { value = "OR" , text = "OR - Oregon"},
                    new { value = "PA" , text = "PA - Pennsylvania"},
                    new { value = "RI" , text = "RI - Rhode Island"},
                    new { value = "SC" , text = "SC - South Carolina"},
                    new { value = "SD" , text = "SD - South Dakota"},
                    new { value = "TN" , text = "TN - Tennessee"},
                    new { value = "TX" , text = "TX - Texas"},
                    new { value = "UT" , text = "UT - Utah"},
                    new { value = "VA" , text = "VA - Virginia"},
                    new { value = "VT" , text = "VT - Vermont"},
                    new { value = "WA" , text = "WA - Washington"},
                    new { value = "WV" , text = "WV - West Virginia"},
                    new { value = "WI" , text = "WI - Wisconsin"},
                    new { value = "WY" , text = "WY - Wyoming"},
                    new { value = "AB" , text = "AB - Alberta"},
                    new { value = "BC" , text = "BC - British Columbia"},
                    new { value = "MB" , text = "MB - Manitoba"},
                    new { value = "NB" , text = "NB - New Brunswick"},
                    new { value = "NL" , text = "NL - Newfoundland"},
                    new { value = "NS" , text = "NS - Nova Scotia"},
                    new { value = "NU" , text = "NU - Nunavut"},
                    new { value = "ON" , text = "ON - Ontario"},
                    new { value = "PE" , text = "PE - Prince Edward Island"},
                    new { value = "QC" , text = "QC - Quebec"},
                    new { value = "SK" , text = "SK - Saskatchewan"},
                    new { value = "NT" , text = "NT - Northwest Territories"},
                    new { value = "YT" , text = "YT - Yukon Territory"}     
                }, "value", "text", defaultStateValue); 
            }
        }
    }
}