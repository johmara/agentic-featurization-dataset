using Microsoft.EntityFrameworkCore;
using ReferenceManager.Models;

namespace ReferenceManager.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Paper> Papers => Set<Paper>();
    public DbSet<Collection> Collections => Set<Collection>(); // &line[Collections]
    public DbSet<Tag> Tags => Set<Tag>(); // &line[Tags]

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Paper>().OwnsMany(p => p.Authors, a =>
        {
            a.ToJson();
            a.OwnsMany(x => x.Affiliations);
        });

        // &begin[Collections]
        modelBuilder.Entity<Collection>()
            .HasMany(c => c.Papers)
            .WithMany(p => p.Collections)
            .UsingEntity("PaperCollection");
        // &end[Collections]

        // &begin[Tags]
        modelBuilder.Entity<Tag>()
            .HasMany(t => t.Papers)
            .WithMany(p => p.Tags)
            .UsingEntity("PaperTag");
        // &end[Tags]
    }
}
