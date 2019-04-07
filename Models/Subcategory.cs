using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exchaggle.Models
{
    public class Subcategory
    {
        [Key]
        public int SubcategoryId { get; set; }
        [ForeignKey("_Category")]
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public virtual Category _Category { get; set; }
    }
}