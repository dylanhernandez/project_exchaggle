using System.ComponentModel.DataAnnotations;

namespace Exchaggle.ViewModels.Items
{
    public class ItemBulletinViewModel
    {
        public ItemBulletinViewModel()
        {
            Status = 0;
        }

        public int ItemId { get; set; }
        [Display(Name ="Title")]
        public string Name { get; set; }
        public string Caption { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public string ImageSource { get; set; }
    }
}