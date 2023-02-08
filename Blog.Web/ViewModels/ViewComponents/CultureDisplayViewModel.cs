using System.Globalization;

namespace Blog.Web.ViewModels.ViewComponents;

public class CultureDisplayViewModel
{
    public CultureInfo Culture { get; set; } = new CultureInfo("en");
}