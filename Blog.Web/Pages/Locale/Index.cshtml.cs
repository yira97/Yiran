using System.Globalization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Blog.Web.Pages.Locale;

public class Index : PageModel
{
    private readonly IOptions<RequestLocalizationOptions> _localizationOptions;

    public IList<CultureInfo> SupportLanguages { get; set; } = new List<CultureInfo>();

    public Index(IOptions<RequestLocalizationOptions> localizationOptions)
    {
        _localizationOptions = localizationOptions;
    }

    public void OnGet()
    {
        SupportLanguages = _localizationOptions.Value.SupportedUICultures!;
    }
}