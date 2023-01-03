using Blog.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Data;

public class ApplicationDbContext : DbContext
{
    // If 'AddDbContext' is used, then also ensure that your DbContext type accepts a DbContextOptions<TContext> object
    // in its constructor and passes it to the base constructor for DbContext
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<PostEntity> Posts => Set<PostEntity>();

    public DbSet<DomainEntity> Domains => Set<DomainEntity>();

    public DbSet<StaticResourceEntity> StaticResources => Set<StaticResourceEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PostEntity>().OwnsOne(
            post => post.Content,
            ownedNavigationBuilder =>
            {
                ownedNavigationBuilder.OwnsOne(content => content.Cover);
                ownedNavigationBuilder.OwnsMany(content => content.Blocks, builder =>
                {
                    builder.OwnsOne(block => block.Image);
                    builder.OwnsMany(block => block.Images);
                });
            });
    }
}