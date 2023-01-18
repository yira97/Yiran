using Blog.Admin.Models;
using Blog.Domain.Services.Client;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Admin.ViewComponents;

public class DomainSelectionViewComponent : ViewComponent
{
    private readonly BlogService _blogService;

    public DomainSelectionViewComponent(BlogService blogService)
    {
        _blogService = blogService;
    }

    public async Task<IViewComponentResult> InvokeAsync(
        string currentDomainId)
    {
        var domains = await _blogService.ListDomains();

        var vm = new DomainSelectionViewModel
        {
            CurrentDomainId = currentDomainId,
            Domains = domains
        };

        return View(vm);
    }
}