using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exchaggle.Models
{
    public class Report
    {
        [Key]
        public int ReportId { get; set; }
        public int ReportableId { get; set; }
        public int ReportableType { get; set; }
        public string Description { get; set; }
        [ForeignKey("_Account")]
        public int AccountId { get; set; }
        public virtual Account _Account { get; set; }
    }
}