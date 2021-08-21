using Microsoft.EntityFrameworkCore;

namespace BlogSample;
public class BlogDbContext : DbContext
{
    public BlogDbContext(DbContextOptions<BlogDbContext> options)
        : base(options)
    {

    }

    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Blog>()
            .HasIndex(b => b.Name)
            .IsUnique();

        modelBuilder.Entity<Post>()
            .HasIndex(p => new { p.BlogId, p.Slug })
            .IsUnique();
    }
}
