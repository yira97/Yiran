using Blog.Api.Data;
using Blog.Api.Repositories;

namespace Blog.Api.Services;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PostRepository> _postRepositoryLogger;
    private readonly ILogger<StaticResourceRepository> _staticResourceRepositoryLogger;
    private readonly ILogger<AccountRepository> _accountRepositoryLogger;

    public UnitOfWork(
        ApplicationDbContext context,
        ILogger<PostRepository> postRepositoryLogger,
        ILogger<StaticResourceRepository> staticResourceRepositoryLogger,
        ILogger<AccountRepository> accountRepositoryLogger)
    {
        _context = context;
        _postRepositoryLogger = postRepositoryLogger;
        _staticResourceRepositoryLogger = staticResourceRepositoryLogger;
        _accountRepositoryLogger = accountRepositoryLogger;
    }

    public IPostRepository PostRepository => new PostRepository(_context, _postRepositoryLogger);

    public IStaticResourceRepository StaticResourceRepository =>
        new StaticResourceRepository(_context, _staticResourceRepositoryLogger);

    public IAccountRepository AccountRepository => new AccountRepository(_context, _accountRepositoryLogger);

    public async Task<bool> CompleteAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}