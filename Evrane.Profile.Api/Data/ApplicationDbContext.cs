using Microsoft.EntityFrameworkCore;

namespace Evrane.Profile.Api.Data;

public class ApplicationDbContext : DbContext
{
    // If 'AddDbContext' is used, then also ensure that your DbContext type accepts a DbContextOptions<TContext> object
    // in its constructor and passes it to the base constructor for DbContext
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}