using System.ComponentModel.DataAnnotations;

namespace Exchaggle.ViewModels.Search
{
    public class SearchResultItemViewModel
    {
        public int ItemId { get; set; }
        [Display(Name="Title")]
        public string Name { get; set; }
        public string Caption { get; set; }
        public string Description { get; set; }
        public bool IsYours { get; set; }
        public string ImageSource { get; set; }
    }
}