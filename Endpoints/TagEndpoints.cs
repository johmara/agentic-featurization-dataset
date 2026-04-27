using Microsoft.EntityFrameworkCore;
using ReferenceManager.Data;
using ReferenceManager.Models;
using ReferenceManager.Requests;
using ReferenceManager.Responses;

namespace ReferenceManager.Endpoints;

public static class TagEndpoints
{
    public static void MapTagEndpoints(this IEndpointRouteBuilder app)
    {
        // &begin[GetTag]
        app.MapGet("/tags", async (AppDbContext db, int limit = 25, int offset = 0) =>
        {
            var total = await db.Tags.CountAsync();
            var items = await db.Tags.AsNoTracking().Include(t => t.Papers)
                .Skip(offset).Take(limit).ToListAsync();
            return Results.Ok(new PagedResult<Tag>(items, total, limit, offset));
        })
        .WithName("ListTags")
        .WithOpenApi();

        app.MapGet("/tags/{id:int}", async (int id, AppDbContext db) =>
            await db.Tags.AsNoTracking().Include(t => t.Papers).FirstOrDefaultAsync(t => t.Id == id) is Tag tag
                ? Results.Ok(tag)
                : Results.NotFound())
            .WithName("GetTag")
            .WithOpenApi();
        // &end[GetTag]

        // &begin[CreateTag]
        app.MapPost("/tags", async (TagRequest req, AppDbContext db) =>
        {
            var tag = new Tag { Name = req.Name };
            db.Tags.Add(tag);
            await db.SaveChangesAsync();
            return Results.Created($"/api/v1/tags/{tag.Id}", tag);
        })
        .WithName("CreateTag")
        .WithOpenApi();
        // &end[CreateTag]

        // &begin[DeleteTag]
        app.MapDelete("/tags/{id:int}", async (int id, AppDbContext db) =>
        {
            var tag = await db.Tags.FindAsync(id);
            if (tag is null) return Results.NotFound();

            db.Tags.Remove(tag);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithName("DeleteTag")
        .WithOpenApi();
        // &end[DeleteTag]

        // &begin[AddTagToPaper]
        app.MapPost("/tags/{id:int}/papers/{paperId:int}", async (int id, int paperId, AppDbContext db) =>
        {
            var tag = await db.Tags.Include(t => t.Papers).FirstOrDefaultAsync(t => t.Id == id);
            if (tag is null) return Results.NotFound();

            var paper = await db.Papers.FindAsync(paperId);
            if (paper is null) return Results.NotFound();

            if (tag.Papers.Any(p => p.Id == paperId)) return Results.Conflict();

            tag.Papers.Add(paper);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithName("AddTagToPaper")
        .WithOpenApi();
        // &end[AddTagToPaper]

        // &begin[RemoveTagFromPaper]
        app.MapDelete("/tags/{id:int}/papers/{paperId:int}", async (int id, int paperId, AppDbContext db) =>
        {
            var tag = await db.Tags.Include(t => t.Papers).FirstOrDefaultAsync(t => t.Id == id);
            if (tag is null) return Results.NotFound();

            var paper = tag.Papers.FirstOrDefault(p => p.Id == paperId);
            if (paper is null) return Results.NotFound();

            tag.Papers.Remove(paper);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithName("RemoveTagFromPaper")
        .WithOpenApi();
        // &end[RemoveTagFromPaper]
    }
}
