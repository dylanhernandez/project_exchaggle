using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exchaggle.Models
{
    public class ItemDetail
    {
        public int ItemDetailId { get; set; }
        [ForeignKey("_Item")]
        public int ItemId { get; set; }
        public int ItemStatus { get; set; }
        public int Reported { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? UploadDate { get; set; }
        public virtual Item _Item { get; set; }
    }
}