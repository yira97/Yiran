using Blog.Admin.Enums;

namespace Blog.Admin.Models;

public class ActionResultViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public ActionResultType? ResultType { get; set; }
    public List<ActionButtonInfoDto> Buttons { get; set; } = new ();
}