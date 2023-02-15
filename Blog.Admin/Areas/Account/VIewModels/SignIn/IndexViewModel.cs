using System.ComponentModel.DataAnnotations;

namespace Blog.Admin.Areas.Account.VIewModels.SignIn;

public class IndexViewModel
{
    public class Input
    {
        [Display(Name = "邮箱")] [EmailAddress] public string Email { get; set; } = string.Empty;

        [Display(Name = "密码")]
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }

    public Input FormInput { get; set; } = new Input();

    public bool AcceptRegister { get; set; } = false;
}