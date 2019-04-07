using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exchaggle.Models
{
    public class AccountDetail
    {
        [Key]
        public int AccountDetailId { get; set; }
        [ForeignKey("_Account")]
        public int AccountId { get; set; }
        public int AccountStatus { get; set; }
        public int AccountLevel { get; set; }
        public string SecurityQuestionA { get; set; }
        public string SecurityQuestionB { get; set; }
        public string SecurityAnswerA { get; set; }
        public string SecurityAnswerB { get; set; }
        public virtual Account _Account { get; set; }
    }
}
