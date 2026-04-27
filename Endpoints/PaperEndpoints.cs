using Microsoft.EntityFrameworkCore;
using ReferenceManager.Data;
using ReferenceManager.Models;
using ReferenceManager.Requests;
using ReferenceManager.Responses;
using ReferenceManager.Services;

namespace ReferenceManager.Endpoints;

public static class PaperEndpoints
{
    public static void MapPaperEndpoints(this IEndpointRouteBuilder app)
    {
        // &begin[GetPaper]
        app.MapGet("/papers", async (AppDbContext db, int limit = 25, int offset = 0) =>
        {
            var total = await db.Papers.CountAsync();
            var items = await db.Papers.Skip(offset).Take(limit).ToListAsync();
            return Results.Ok(new PagedResult<Paper>(items, total, limit, offset));
        })
        .WithName("ListPapers")
        .WithOpenApi();

        app.MapGet("/papers/{id:int}", async (int id, AppDbContext db) =>
            await db.Papers.FindAsync(id) is Paper paper
                ? Results.Ok(paper)
                : Results.NotFound())
            .WithName("GetPaper")
            .WithOpenApi();
        // &end[GetPaper]

        // &begin[CreatePaper]
        app.MapPost("/papers", async (PaperRequest req, AppDbContext db) =>
        {
            var paper = new Paper
            {
                Title = req.Title,
                Authors = req.Authors.Select(ToAuthor).ToList(),
                Year = req.Year,
                Abstract = req.Abstract,
                Doi = req.Doi,
                Journal = req.Journal,
                Booktitle = req.Booktitle,
            };
            db.Papers.Add(paper);
            await db.SaveChangesAsync();
            return Results.Created($"/api/v1/papers/{paper.Id}", paper);
        })
        .WithName("CreatePaper")
        .WithOpenApi();
        // &end[CreatePaper]

        // &begin[UpdatePaper]
        app.MapPut("/papers/{id:int}", async (int id, PaperRequest req, AppDbContext db) =>
        {
            var paper = await db.Papers.FindAsync(id);
            if (paper is null) return Results.NotFound();

            paper.Title = req.Title;
            paper.Authors = req.Authors.Select(ToAuthor).ToList();
            paper.Year = req.Year;
            paper.Abstract = req.Abstract;
            paper.Doi = req.Doi;
            paper.Journal = req.Journal;
            paper.Booktitle = req.Booktitle;
            await db.SaveChangesAsync();
            return Results.Ok(paper);
        })
        .WithName("UpdatePaper")
        .WithOpenApi();
        // &end[UpdatePaper]

        // &begin[DeletePaper]
        app.MapDelete("/papers/{id:int}", async (int id, AppDbContext db) =>
        {
            var paper = await db.Papers.FindAsync(id);
            if (paper is null) return Results.NotFound();

            db.Papers.Remove(paper);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithName("DeletePaper")
        .WithOpenApi();
        // &end[DeletePaper]

        // &begin[ImportBibtex]
        app.MapPost("/papers/import/bibtex", async (IFormFile file, AppDbContext db) =>
        {
            using var reader = new StreamReader(file.OpenReadStream());
            var content = await reader.ReadToEndAsync();
            var entries = BibtexParser.Parse(content);

            int created = 0, skipped = 0;
            var seenDois = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var seenTitleAuthor = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var entry in entries)
            {
                if (!entry.Fields.TryGetValue("title", out var title) || string.IsNullOrWhiteSpace(title))
                    continue;

                entry.Fields.TryGetValue("doi", out var doi);
                var normalizedDoi = string.IsNullOrWhiteSpace(doi) ? null : doi.Trim();

                if (normalizedDoi is not null &&
                    (seenDois.Contains(normalizedDoi) || await db.Papers.AnyAsync(p => p.Doi == normalizedDoi)))
                {
                    skipped++;
                    continue;
                }

                if (normalizedDoi is not null) seenDois.Add(normalizedDoi);

                entry.Fields.TryGetValue("author", out var authorStr);
                var authorNames = (authorStr ?? string.Empty)
                    .Split(" and ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .ToList();
                var authors = authorNames.Select(name => new Author { Name = name }).ToList();

                if (normalizedDoi is null)
                {
                    var titleAuthorKey = TitleAuthorKey(title, authorNames);
                    if (seenTitleAuthor.Contains(titleAuthorKey))
                    {
                        skipped++;
                        continue;
                    }
                    var normalizedTitle = title.Trim().ToLowerInvariant();
                    var candidates = await db.Papers
                        .Where(p => p.Doi == null && p.Title.ToLower() == normalizedTitle)
                        .ToListAsync();
                    if (candidates.Any(p => TitleAuthorKey(p.Title, p.Authors.Select(a => a.Name)) == titleAuthorKey))
                    {
                        skipped++;
                        continue;
                    }
                    seenTitleAuthor.Add(titleAuthorKey);
                }

                int.TryParse(entry.Fields.GetValueOrDefault("year"), out var year);
                entry.Fields.TryGetValue("abstract", out var abstractStr);
                entry.Fields.TryGetValue("journal", out var journal);
                entry.Fields.TryGetValue("booktitle", out var booktitle);

                db.Papers.Add(new Paper
                {
                    Title = title,
                    Authors = authors,
                    Year = year,
                    Abstract = string.IsNullOrWhiteSpace(abstractStr) ? null : abstractStr,
                    Doi = normalizedDoi,
                    Journal = string.IsNullOrWhiteSpace(journal) ? null : journal,
                    Booktitle = string.IsNullOrWhiteSpace(booktitle) ? null : booktitle,
                });
                created++;
            }

            await db.SaveChangesAsync();
            return Results.Ok(new ImportResult(created, skipped));
        })
        .DisableAntiforgery()
        .WithName("ImportBibtex")
        .WithOpenApi();
        // &end[ImportBibtex]

        // &begin[ExportBibtex]
        app.MapGet("/papers/export/bibtex", async (AppDbContext db, int? groupId, bool? favorited) =>
        {
            var query = db.Papers.Include(p => p.Authors).AsQueryable();

            if (groupId.HasValue)
                query = query.Where(p => p.Groups.Any(g => g.Id == groupId.Value));

            if (favorited == true)
                query = query.Where(p => p.IsFavorited);

            var papers = await query.ToListAsync();
            var bib = BibtexSerializer.Serialize(papers);

            return Results.Text(bib, "text/plain; charset=utf-8");
        })
        .WithName("ExportBibtex")
        .Produces<string>(200, "text/plain")
        .WithOpenApi();
        // &end[ExportBibtex]
    }

    private static string TitleAuthorKey(string title, IEnumerable<string> authorNames) =>
        title.Trim().ToLowerInvariant() + "||" +
        string.Join("|", authorNames.Select(n => n.Trim().ToLowerInvariant()).Order());

    private static Author ToAuthor(AuthorRequest r) => new()
    {
        Name = r.Name,
        Email = r.Email,
        Affiliations = r.Affiliations
            .Select(a => new Affiliation { Name = a.Name, Country = a.Country, City = a.City })
            .ToList()
    };
}
