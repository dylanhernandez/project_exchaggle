using Exchaggle.ViewModels.Items;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Exchaggle.ViewModels.Trades
{
    public class MakeOfferViewModel
    {
        public int ReferenceId { get; set; }
        [Display(Name = "Title")]
        public string Name { get; set; }
        public string Caption { get; set; }
        public string Description { get; set; }
        public IEnumerable<ItemBulletinViewModel> OfferOptions { get; set; }
    }
}