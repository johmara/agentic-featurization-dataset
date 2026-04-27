using ReferenceManager.Models;

namespace ReferenceManager.Responses;

// &begin[Authors]
public record AuthorSummary(int Id, string Name, string? Email, List<Affiliation> Affiliations);
// &end[Authors]

// &begin[Groups]
public record GroupSummary(int Id, string Name, string? Description);
// &end[Groups]

public record PaperResponse(
    int Id,
    string Title,
    List<AuthorSummary> Authors, // &line[Authors]
    int Year,
    string? Abstract,
    string? Doi,
    string? Journal, // &line[ImportBibtex]
    string? Booktitle, // &line[ImportBibtex]
    List<GroupSummary> Groups // &line[Groups]
);

public static class PaperResponseExtensions
{
    public static PaperResponse ToResponse(this Paper p) => new(
        p.Id,
        p.Title,
        p.Authors.Select(a => new AuthorSummary(a.Id, a.Name, a.Email, a.Affiliations)).ToList(),
        p.Year,
        p.Abstract,
        p.Doi,
        p.Journal,
        p.Booktitle,
        p.Groups.Select(g => new GroupSummary(g.Id, g.Name, g.Description)).ToList()
    );
}
