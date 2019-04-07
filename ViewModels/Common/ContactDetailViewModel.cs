using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Exchaggle.ViewModels.Common
{
    public class ContactDetailViewModel
    {
        public string Email { get; set; }
        public string ContactName { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
    }
}