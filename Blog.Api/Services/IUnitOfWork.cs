using Blog.Api.Repositories;

namespace Blog.Api.Services;

public interface IUnitOfWork
{
    IPostRepository PostRepository { get; }
    
    IStaticResourceRepository StaticResourceRepository { get; }
    
    Task<bool> CompleteAsync();
}