using Blog.Api.Repositories;

namespace Blog.Api.Services;

public interface IUnitOfWork
{
    IPostRepository PostRepository { get; }
    
    Task<bool> CompleteAsync();
}