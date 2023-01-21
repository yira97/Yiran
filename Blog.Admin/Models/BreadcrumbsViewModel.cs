namespace Blog.Admin.Models;

public class BreadcrumbsViewModel
{
    public BreadcrumbsViewModel(BreadcrumbsDto breadcrumbs)
    {
        this.Breadcrumbs = breadcrumbs;
    }

    public BreadcrumbsDto Breadcrumbs { get; set; }
}