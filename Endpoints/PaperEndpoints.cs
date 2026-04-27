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
            var items = await db.Papers.Include(p => p.Authors).Include(p => p.Groups)
                .Skip(offset).Take(limit).ToListAsync();
            return Results.Ok(new PagedResult<PaperResponse>(items.Select(p => p.ToResponse()).ToList(), total, limit, offset));
        })
        .WithName("ListPapers")
        .WithOpenApi();

        app.MapGet("/papers/{id:int}", async (int id, AppDbContext db) =>
            await db.Papers.Include(p => p.Authors).Include(p => p.Groups).FirstOrDefaultAsync(p => p.Id == id) is Paper paper
                ? Results.Ok(paper.ToResponse())
                : Results.NotFound())
            .WithName("GetPaper")
            .WithOpenApi();
        // &end[GetPaper]

        // &begin[CreatePaper]
        app.MapPost("/papers", async (PaperRequest req, AppDbContext db) =>
        {
            var cache = new Dictionary<string, Author>();
            var authors = new List<Author>();
            foreach (var a in req.Authors)
                authors.Add(await AuthorEndpoints.GetOrCreateAsync(a.Name, a.Email, a.Affiliations, db, cache));

            var paper = new Paper
            {
                Title = req.Title,
                Authors = authors,
                Year = req.Year,
                Abstract = req.Abstract,
                Doi = req.Doi,
                Journal = req.Journal,
                Booktitle = req.Booktitle,
            };
            db.Papers.Add(paper);
            await db.SaveChangesAsync();
            return Results.Created($"/api/v1/papers/{paper.Id}", paper.ToResponse());
        })
        .WithName("CreatePaper")
        .WithOpenApi();
        // &end[CreatePaper]

        // &begin[UpdatePaper]
        app.MapPut("/papers/{id:int}", async (int id, PaperRequest req, AppDbContext db) =>
        {
            var paper = await db.Papers.Include(p => p.Authors).Include(p => p.Groups).FirstOrDefaultAsync(p => p.Id == id);
            if (paper is null) return Results.NotFound();

            var cache = new Dictionary<string, Author>();
            var authors = new List<Author>();
            foreach (var a in req.Authors)
                authors.Add(await AuthorEndpoints.GetOrCreateAsync(a.Name, a.Email, a.Affiliations, db, cache));

            paper.Title = req.Title;
            paper.Authors = authors;
            paper.Year = req.Year;
            paper.Abstract = req.Abstract;
            paper.Doi = req.Doi;
            paper.Journal = req.Journal;
            paper.Booktitle = req.Booktitle;
            await db.SaveChangesAsync();
            return Results.Ok(paper.ToResponse());
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
            var authorCache = new Dictionary<string, Author>(StringComparer.OrdinalIgnoreCase);
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

                if (normalizedDoi is null)
                {
                    var titleAuthorKey = TitleAuthorKey(title, authorNames);
                    if (seenTitleAuthor.Contains(titleAuthorKey))
                    {
                        skipped++;
                        continue;
                    }
                    var normalizedTitle = title.Trim().ToLowerInvariant();
                    var candidates = await db.Papers.Include(p => p.Authors)
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

                var authors = new List<Author>();
                foreach (var name in authorNames)
                    authors.Add(await AuthorEndpoints.GetOrCreateAsync(name, null, [], db, authorCache));

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

        // &begin[SearchPapers]
        app.MapGet("/papers/search", async (
            AppDbContext db,
            string? q,
            int? groupId,
            int? yearFrom,
            int? yearTo,
            int limit = 25,
            int offset = 0) =>
        {
            var query = db.Papers.Include(p => p.Authors).Include(p => p.Groups).AsQueryable();

            if (groupId.HasValue)
                query = query.Where(p => p.Groups.Any(g => g.Id == groupId.Value));
            if (yearFrom.HasValue)
                query = query.Where(p => p.Year >= yearFrom.Value);
            if (yearTo.HasValue)
                query = query.Where(p => p.Year <= yearTo.Value);

            var papers = await query.ToListAsync();

            if (string.IsNullOrWhiteSpace(q))
            {
                var page = papers.Skip(offset).Take(limit)
                    .Select(p => new SearchResultItem(p.ToResponse(), 0)).ToList();
                return Results.Ok(new SearchResponse(page, papers.Count));
            }

            var terms = q.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(t => t.ToLowerInvariant()).Distinct().ToArray();

            var scored = papers
                .Select(p =>
                {
                    var title = p.Title.ToLowerInvariant();
                    var abstrakt = p.Abstract?.ToLowerInvariant() ?? "";
                    var authors = p.Authors.Select(a => a.Name.ToLowerInvariant()).ToList();
                    double score = 0;
                    foreach (var term in terms)
                    {
                        if (title.Contains(term)) score += 10;
                        if (abstrakt.Contains(term)) score += 3;
                        foreach (var name in authors)
                            if (name.Contains(term)) score += 5;
                    }
                    return (Paper: p, Score: score);
                })
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .ToList();

            var resultPage = scored.Skip(offset).Take(limit)
                .Select(x => new SearchResultItem(x.Paper.ToResponse(), x.Score)).ToList();
            return Results.Ok(new SearchResponse(resultPage, scored.Count));
        })
        .WithName("SearchPapers")
        .WithOpenApi();
        // &end[SearchPapers]

        // &begin[ExportBibtex]
        app.MapGet("/papers/export/bibtex", async (AppDbContext db, int? groupId) =>
        {
            var query = db.Papers.Include(p => p.Authors).AsQueryable();

            if (groupId.HasValue)
                query = query.Where(p => p.Groups.Any(g => g.Id == groupId.Value));

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
}
