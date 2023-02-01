using Blog.Domain.Models;

namespace Blog.Web.Services;

public interface IDomainService
{
    public Task<DomainDto> GetInfo();
}