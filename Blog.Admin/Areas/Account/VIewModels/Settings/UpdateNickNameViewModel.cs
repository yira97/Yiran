using System.ComponentModel.DataAnnotations;

namespace Blog.Admin.Areas.Account.VIewModels.Settings;

public class UpdateNickNameViewModel
{
    public class Input
    {
        public string NickName { get; set; } = string.Empty;
    }

    public Input FormInput { get; set; } = new Input();
}