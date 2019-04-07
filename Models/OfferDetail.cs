using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exchaggle.Models
{
    public class OfferDetail
    {
        [Key]
        public int OfferDetailId { get; set; }
        [ForeignKey("_Offer")]
        public int OfferId { get; set; }
        public int Confirmed { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime ExpirationDate { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime UploadDate { get; set; }
        public virtual Offer _Offer { get; set; }
    }
}