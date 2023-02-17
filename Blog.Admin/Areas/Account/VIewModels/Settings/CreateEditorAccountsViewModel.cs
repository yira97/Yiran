namespace Blog.Admin.Areas.Account.VIewModels.Settings;

public class CreateEditorAccountsViewModel
{
    public class Input
    {
        public string Email { get; set; } = string.Empty;
    }

    public Input FormInput { get; set; } = new Input();
}