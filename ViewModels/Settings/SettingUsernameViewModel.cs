using System.ComponentModel.DataAnnotations;

namespace Exchaggle.ViewModels.Settings
{
    public class SettingUsernameViewModel
    {
        [Required(ErrorMessage = "Your current username is required")]
        public string OldUsername { get; set; }
        [Required(ErrorMessage = "New username is required")]
        public string NewUsername { get; set; }
    }
}