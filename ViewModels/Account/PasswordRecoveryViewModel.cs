using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Exchaggle.ViewModels
{
    public class PasswordRecoveryViewModel
    {
        [Required(ErrorMessage = "Email is required for secure verification")]
        [EmailAddress(ErrorMessage = "Email is invalid")]
        [Display(Name = "Email")]
        public string EmailAddress { get; set; }
        public string SecurityQuestionA { get; set; }
        public string SecurityQuestionB { get; set; }
        public string SecurityAnswerA { get; set; }
        public string SecurityAnswerB { get; set; }
    }
}