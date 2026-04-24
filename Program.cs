using Microsoft.EntityFrameworkCore;
using ReferenceManager.Data;
using ReferenceManager.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// &begin[Database]
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
// &end[Database]

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();

// &begin[Database]
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}
// &end[Database]

// &begin[GetPaper]
app.MapGet("/papers", async (AppDbContext db) =>
    await db.Papers.ToListAsync())
    .WithName("ListPapers");

app.MapGet("/papers/{id:int}", async (int id, AppDbContext db) =>
    await db.Papers.FindAsync(id) is Paper paper
        ? Results.Ok(paper)
        : Results.NotFound())
    .WithName("GetPaper");
// &end[GetPaper]

// &begin[CreatePaper]
app.MapPost("/papers", async (PaperRequest req, AppDbContext db) =>
{
    var paper = new Paper
    {
        Title = req.Title,
        Authors = req.Authors,
        Year = req.Year,
        Abstract = req.Abstract,
        Doi = req.Doi
    };
    db.Papers.Add(paper);
    await db.SaveChangesAsync();
    return Results.Created($"/papers/{paper.Id}", paper);
})
.WithName("CreatePaper");
// &end[CreatePaper]

// &begin[UpdatePaper]
app.MapPut("/papers/{id:int}", async (int id, PaperRequest req, AppDbContext db) =>
{
    var paper = await db.Papers.FindAsync(id);
    if (paper is null) return Results.NotFound();

    paper.Title = req.Title;
    paper.Authors = req.Authors;
    paper.Year = req.Year;
    paper.Abstract = req.Abstract;
    paper.Doi = req.Doi;
    await db.SaveChangesAsync();
    return Results.Ok(paper);
})
.WithName("UpdatePaper");
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
.WithName("DeletePaper");
// &end[DeletePaper]

app.Run();

record PaperRequest(string Title, List<string> Authors, int Year, string? Abstract, string? Doi);
