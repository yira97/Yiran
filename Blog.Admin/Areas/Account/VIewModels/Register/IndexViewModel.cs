using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Blog.Admin.Areas.Account.VIewModels.Register;

public class IndexViewModel
{
    public class Input
    {
        [Required(ErrorMessage = "Please enter email")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter email")]
        [MinLength(6, ErrorMessage = "Minimum length is 6")]
        [PasswordPropertyText]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter password")]
        [Compare("Password", ErrorMessage = "The two passwords must be the same")]
        [PasswordPropertyText]
        public string PasswordConfirmation { get; set; } = string.Empty;
    }

    public Input FormInput { get; set; } = new Input();
}