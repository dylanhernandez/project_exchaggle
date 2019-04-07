using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Exchaggle.Models
{
    public class Category
    {
        public Category ()
        {
            Subcategories = new List<Subcategory>();
        }

        [Key]
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Subcategory> Subcategories { get; set; }
    }
}