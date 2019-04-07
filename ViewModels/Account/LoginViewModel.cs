using System.ComponentModel.DataAnnotations;

namespace Exchaggle.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email/Username is required")]
        [Display(Name = "Email or Username")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}