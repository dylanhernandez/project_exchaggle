using Exchaggle.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Exchaggle.ViewModels.Trades
{
    public class TradesDetailViewModel
    {
        public int OfferId { get; set; }
        public int Confirmed { get; set; }
        public DateTime ExpirationDate { get; set; }
        public ItemsDetailViewModel ReceiverItem { get; set; }
        public ItemsDetailViewModel SenderItem { get; set; }
        public ContactDetailViewModel ReceiverDetails { get; set; }
        public ContactDetailViewModel SenderDetails { get; set; }
    }
}