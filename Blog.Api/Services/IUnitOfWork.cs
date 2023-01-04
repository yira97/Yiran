using Blog.Api.Repositories;

namespace Blog.Api.Services;

public interface IUnitOfWork
{
    IPostRepository PostRepository { get; }

    IStaticResourceRepository StaticResourceRepository { get; }

    IAccountRepository AccountRepository { get; }

    Task<bool> CompleteAsync();
}