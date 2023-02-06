using System.Reflection;
using Blog.Web.Resources;
using Microsoft.Extensions.Localization;

namespace Blog.Web.Services;

public class CommonLocalizationService
{
    private readonly IStringLocalizer _localizer;

    public CommonLocalizationService(IStringLocalizerFactory factory)
    {
        var type = typeof(CommonResources);
        _localizer = factory.Create(type);
    }

    public LocalizedString Get(string key)
    {
        return _localizer[key];
    }

    public LocalizedString GetWith(string key, string parameter)
    {
        return _localizer[key, parameter];
    }
}