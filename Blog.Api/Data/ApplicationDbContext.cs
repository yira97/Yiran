using Blog.Api.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUserEntity, ApplicationRoleEntity, string>
{
    // If 'AddDbContext' is used, then also ensure that your DbContext type accepts a DbContextOptions<TContext> object
    // in its constructor and passes it to the base constructor for DbContext
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<PostEntity> Posts => Set<PostEntity>();

    public DbSet<DomainEntity> Domains => Set<DomainEntity>();

    public DbSet<DomainCategoryEntity> DomainCategories => Set<DomainCategoryEntity>();

    public DbSet<DomainTopicEntity> DomainTopics => Set<DomainTopicEntity>();

    public DbSet<StaticResourceEntity> StaticResources => Set<StaticResourceEntity>();

    public DbSet<SiteMapEntity> DomainSiteMaps => Set<SiteMapEntity>();

    public DbSet<TextContentEntity> TextContents => Set<TextContentEntity>();

    public DbSet<TextContentTranslationEntity> TextContentTranslations => Set<TextContentTranslationEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // call super first
        base.OnModelCreating(modelBuilder);

        // Npgsql
        modelBuilder.Entity<PostEntity>()
            .Property(b => b.Content)
            .HasColumnType("jsonb");

        modelBuilder.Entity<DomainEntity>()
            .Property(d => d.SocialLinks)
            .HasColumnType("jsonb");

        modelBuilder.Entity<DomainCategoryEntity>()
            .HasOne(dc => dc.Domain)
            .WithMany(d => d.Categories)
            .HasForeignKey(dc => dc.DomainId);

        modelBuilder.Entity<DomainTopicEntity>()
            .HasOne(dc => dc.Domain)
            .WithMany(d => d.Topics)
            .HasForeignKey(dc => dc.DomainId);

        modelBuilder.Entity<DomainEntity>()
            .HasOne(d => d.SiteMap)
            .WithOne(sm => sm.Domain)
            .HasForeignKey<DomainEntity>(d => d.SiteMapId);
    }
}