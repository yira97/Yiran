using Blog.Api.Data;
using Blog.Api.Repositories;

namespace Blog.Api.Services;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PostRepository> _postRepositoryLogger;
    private readonly ILogger<StaticResourceRepository> _staticResourceRepositoryLogger;

    public UnitOfWork(
            ApplicationDbContext context,
            ILogger<PostRepository> postRepositoryLogger,
            ILogger<StaticResourceRepository> staticResourceRepositoryLogger
            )
    {
        _context = context;
        _postRepositoryLogger = postRepositoryLogger;
        _staticResourceRepositoryLogger = staticResourceRepositoryLogger;
    }

    public IPostRepository PostRepository => new PostRepository(_context, _postRepositoryLogger);

    public IStaticResourceRepository StaticResourceRepository => new StaticResourceRepository(_context, _staticResourceRepositoryLogger);
    
    public async Task<bool> CompleteAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}