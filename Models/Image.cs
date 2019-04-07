using System.ComponentModel.DataAnnotations.Schema;

namespace Exchaggle.Models
{
    public class Image
    {
        public int ImageId { get; set; }
        [ForeignKey("_Item")]
        public int ItemId { get; set; }
        public string ImageName { get; set; }
        public string ImageSource { get; set; }
        public virtual Item _Item { get; set; }
    }
}