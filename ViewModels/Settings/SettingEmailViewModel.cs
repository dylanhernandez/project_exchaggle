using System.ComponentModel.DataAnnotations;

namespace Exchaggle.ViewModels.Settings
{
    public class SettingEmailViewModel
    {
        [Required(ErrorMessage = "Your current email address is required")]
        [EmailAddress(ErrorMessage = "Email is invalid")]
        public string OldEmail { get; set; }

        [Required(ErrorMessage = "New email address is required")]
        [EmailAddress(ErrorMessage = "Email is invalid")]
        public string NewEmail { get; set; }
    }
}