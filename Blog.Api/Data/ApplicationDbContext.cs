using Blog.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Data;

public class ApplicationDbContext: DbContext
{
    public DbSet<PostEntity> Posts => Set<PostEntity>();

    public DbSet<DomainEntity> Domains => Set<DomainEntity>();

    public DbSet<StaticResourceEntity> StaticResources => Set<StaticResourceEntity>();
}