using System.ComponentModel.DataAnnotations;

namespace Blog.Admin.Areas.Account.VIewModels.SignIn;

public class IndexViewModel
{
    public class Input
    {
        [EmailAddress] public string Email { get; set; } = string.Empty;

        [Required] [MinLength(6)] public string Password { get; set; } = string.Empty;
    }

    public Input FormInput { get; set; } = new Input();

    public bool AcceptRegister { get; set; } = false;
}