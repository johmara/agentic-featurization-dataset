using Microsoft.EntityFrameworkCore;
using ReferenceManager.Models;

namespace ReferenceManager.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Paper> Papers => Set<Paper>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Paper>().OwnsMany(p => p.Authors, a =>
        {
            a.ToJson();
            a.OwnsMany(x => x.Affiliations);
        });
    }
}
