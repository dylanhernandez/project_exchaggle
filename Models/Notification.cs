using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exchaggle.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }
        [ForeignKey("_Account")]
        public int AccountId { get; set; }
        public string Description { get; set; }
        public string Action { get; set; }
        public string Contorller { get; set; }
        public string Reference { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? UploadDate { get; set; }
        public virtual Account _Account { get; set; }
    }
}