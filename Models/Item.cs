using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exchaggle.Models
{
    public class Item
    {
        [Key]
        public int ItemId { get; set; }
        [ForeignKey("_Account")]
        public int AccountId { get; set; }
        public string Name { get; set; }
        public string Caption { get; set; }
        public string Description { get; set; }
        [ForeignKey("_Category")]
        public int CategoryId { get; set; }
        public int SubcategoryId { get; set; }
        public int TradeCategoryId { get; set; }
        public int TradeSubcategoryId { get; set; }
        public virtual Account _Account { get; set; }
        public virtual Category _Category { get; set; }
    }
}