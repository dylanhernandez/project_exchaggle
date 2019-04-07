using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Exchaggle.ViewModels.Common
{
    public class ItemsDetailViewModel
    {
        public ItemsDetailViewModel()
        {
            Status = 0;
        }

        public int ItemId { get; set; }
        [Display(Name = "Title")]
        public string Name { get; set; }
        public string Caption { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string SubcategoryName { get; set; }
        public string TradeCategoryName { get; set; }
        public string TradeSubcategoryName { get; set; }
        public string PostedBy { get; set; }
        public bool InWishList { get; set; }
        public int Status { get; set; }
        public string ImageString { get; set; }
    }
}