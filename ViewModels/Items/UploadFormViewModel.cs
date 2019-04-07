using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace Exchaggle.ViewModels.Items
{
    public class UploadFormViewModel
    {
        public UploadFormViewModel()
        {
            ImageKeep = 0; //Handler for saved images, 0 means erase the currently held image, 1 means to erase
        }

        public SelectList CategoriesList { get; set; }
        public SelectList SubcategoriesList { get; set; }

        [Required(ErrorMessage = "Please provide a title for what you are uploading")]
        [Display(Name = "Title")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Your item name must be between 1 - 100 characters long")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please provide a caption for what you are uploading")]
        [Display(Name = "Caption")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Your item caption must be between 1 - 100 characters long")]
        public string Caption { get; set; }
        [Required(ErrorMessage = "Please provide a description for what you are uploading")]
        [Display(Name = "Description")]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Your description must be between 1 - 1000 characters long")]
        public string Description { get; set; }

        [DataType(DataType.Upload)]
        public HttpPostedFileBase ImageUpload { get; set; }
        public int ImageKeep { get; set; }

        public int ItemCategory { get; set; }
        public int ItemSubcategory { get; set; }
        public int TradeCategory { get; set; }
        public int TradeSubcategory { get; set; }
        public string ReferenceAction { get; set; }
        public int ReferenceId { get; set; }

        public string ImageString { get; set; }
    }
}