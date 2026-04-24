using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ReferenceManager.Models;

namespace ReferenceManager.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Paper> Papers => Set<Paper>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var authorsComparer = new ValueComparer<List<string>>(
            (a, b) => a != null && b != null && a.SequenceEqual(b),
            v => v.Aggregate(0, (a, s) => HashCode.Combine(a, s.GetHashCode())),
            v => v.ToList());

        modelBuilder.Entity<Paper>()
            .Property(p => p.Authors)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<List<string>>(v, JsonSerializerOptions.Default) ?? new())
            .Metadata.SetValueComparer(authorsComparer);
    }
}
