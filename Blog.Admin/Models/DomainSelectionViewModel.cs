using Blog.Domain.Models;

namespace Blog.Admin.Models;

public class DomainSelectionViewModel
{
    public string CurrentDomainId { get; set; } = string.Empty;
    public IEnumerable<DomainDto> Domains = new List<DomainDto>();
}