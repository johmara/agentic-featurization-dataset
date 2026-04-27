using Microsoft.EntityFrameworkCore;
using ReferenceManager.Data;
using ReferenceManager.Models;
using ReferenceManager.Requests;
using ReferenceManager.Responses;

namespace ReferenceManager.Endpoints;

public static class GroupEndpoints
{
    public static void MapGroupEndpoints(this IEndpointRouteBuilder app)
    {
        // &begin[GetGroup]
        app.MapGet("/groups", async (AppDbContext db, int limit = 25, int offset = 0) =>
        {
            var total = await db.Groups.CountAsync();
            var items = await db.Groups.AsNoTracking().Include(g => g.Papers)
                .Skip(offset).Take(limit).ToListAsync();
            return Results.Ok(new PagedResult<Group>(items, total, limit, offset));
        })
        .WithName("ListGroups")
        .WithOpenApi();

        app.MapGet("/groups/{id:int}", async (int id, AppDbContext db) =>
            await db.Groups.AsNoTracking().Include(g => g.Papers).FirstOrDefaultAsync(g => g.Id == id) is Group group
                ? Results.Ok(group)
                : Results.NotFound())
            .WithName("GetGroup")
            .WithOpenApi();
        // &end[GetGroup]

        // &begin[CreateGroup]
        app.MapPost("/groups", async (GroupRequest req, AppDbContext db) =>
        {
            var group = new Group { Name = req.Name, Description = req.Description };
            db.Groups.Add(group);
            await db.SaveChangesAsync();
            return Results.Created($"/api/v1/groups/{group.Id}", group);
        })
        .WithName("CreateGroup")
        .WithOpenApi();
        // &end[CreateGroup]

        // &begin[UpdateGroup]
        app.MapPut("/groups/{id:int}", async (int id, GroupRequest req, AppDbContext db) =>
        {
            var group = await db.Groups.FindAsync(id);
            if (group is null) return Results.NotFound();

            group.Name = req.Name;
            group.Description = req.Description;
            await db.SaveChangesAsync();
            return Results.Ok(group);
        })
        .WithName("UpdateGroup")
        .WithOpenApi();
        // &end[UpdateGroup]

        // &begin[DeleteGroup]
        app.MapDelete("/groups/{id:int}", async (int id, AppDbContext db) =>
        {
            var group = await db.Groups.FindAsync(id);
            if (group is null) return Results.NotFound();

            db.Groups.Remove(group);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithName("DeleteGroup")
        .WithOpenApi();
        // &end[DeleteGroup]

        // &begin[AddPaperToGroup]
        app.MapPost("/groups/{id:int}/papers/{paperId:int}", async (int id, int paperId, AppDbContext db) =>
        {
            var group = await db.Groups.Include(g => g.Papers).FirstOrDefaultAsync(g => g.Id == id);
            if (group is null) return Results.NotFound();

            var paper = await db.Papers.FindAsync(paperId);
            if (paper is null) return Results.NotFound();

            if (group.Papers.Any(p => p.Id == paperId)) return Results.Conflict();

            group.Papers.Add(paper);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithName("AddPaperToGroup")
        .WithOpenApi();
        // &end[AddPaperToGroup]

        // &begin[RemovePaperFromGroup]
        app.MapDelete("/groups/{id:int}/papers/{paperId:int}", async (int id, int paperId, AppDbContext db) =>
        {
            var group = await db.Groups.Include(g => g.Papers).FirstOrDefaultAsync(g => g.Id == id);
            if (group is null) return Results.NotFound();

            var paper = group.Papers.FirstOrDefault(p => p.Id == paperId);
            if (paper is null) return Results.NotFound();

            group.Papers.Remove(paper);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithName("RemovePaperFromGroup")
        .WithOpenApi();
        // &end[RemovePaperFromGroup]
    }
}
