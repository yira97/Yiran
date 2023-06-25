using Blog.Api.Data;
using Blog.Api.Repositories;

namespace Blog.Api.Services;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PostRepository> _postRepositoryLogger;
    private readonly ILogger<StaticResourceRepository> _staticResourceRepositoryLogger;
    private readonly ILogger<DomainRepository> _domainRepositoryLogger;

    public UnitOfWork(
        ApplicationDbContext context,
        // ReSharper disable once ContextualLoggerProblem
        ILogger<PostRepository> postRepositoryLogger,
        // ReSharper disable once ContextualLoggerProblem
        ILogger<StaticResourceRepository> staticResourceRepositoryLogger,
        // ReSharper disable once ContextualLoggerProblem
        ILogger<DomainRepository> domainRepositoryLogger
    )
    {
        _context = context;
        _postRepositoryLogger = postRepositoryLogger;
        _staticResourceRepositoryLogger = staticResourceRepositoryLogger;
        _domainRepositoryLogger = domainRepositoryLogger;
    }

    public IPostRepository PostRepository => new PostRepository(_context, _postRepositoryLogger);

    public IStaticResourceRepository StaticResourceRepository =>
        new StaticResourceRepository(_context, _staticResourceRepositoryLogger);

    public IDomainRepository DomainRepository => new DomainRepository(_context, _domainRepositoryLogger);

    public async Task<bool> CompleteAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}