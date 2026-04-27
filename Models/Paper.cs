namespace ReferenceManager.Models;

public class Paper
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<Author> Authors { get; set; } = []; // &line[Authors]
    public int Year { get; set; }
    public string? Abstract { get; set; }
    public string? Doi { get; set; }
    public string? Journal { get; set; } // &line[ImportBibtex]
    public string? Booktitle { get; set; } // &line[ImportBibtex]
    public List<Group> Groups { get; set; } = []; // &line[Groups]
}
