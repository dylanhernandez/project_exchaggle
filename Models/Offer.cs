using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exchaggle.Models
{
    public class Offer
    {
        [Key]
        public int OfferId { get; set; }
        [ForeignKey("_Account")]
        public int AccountId { get; set; }
        public int SenderItemId { get; set; }
        public int ReceiverId { get; set; }
        public int ReceiverItemId { get; set; }
        public virtual Account _Account { get; set; }
    }
}