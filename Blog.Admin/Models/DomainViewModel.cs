using Blog.Domain.Models;

namespace Blog.Admin.Models;

public class DomainViewModel
{
    public List<DomainDto> Domains { get; set; } = new List<DomainDto>();
}