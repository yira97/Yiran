using Microsoft.AspNetCore.Mvc.Rendering;

namespace Blog.Admin.Areas.Account.VIewModels.Settings;

public class UpdateLanguageViewModel
{
    public class Input
    {
        public string Language { get; set; } = string.Empty;
    }

    public Input FormInput { get; set; } = new Input();

    public IEnumerable<SelectListItem> Languages = new List<SelectListItem>();
}