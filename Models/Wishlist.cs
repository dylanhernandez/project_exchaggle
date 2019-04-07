using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Exchaggle.Models
{
    public class Wishlist
    {
        [Key]
        public int WishListId { get; set; }
        public int ItemId { get; set; }
        [ForeignKey("_Account")]
        public int AccountId { get; set; }
        public virtual Account _Account { get; set; }
    }
}