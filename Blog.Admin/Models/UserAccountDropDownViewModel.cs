using Blog.Domain.Models;

namespace Blog.Admin.Models;

public class UserAccountDropDownViewModel
{
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;

    public List<NavigationDto> UserNavigation { get; set; } = new();
}