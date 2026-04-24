namespace ReferenceManager.Models;

public class Paper
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<Author> Authors { get; set; } = [];
    public int Year { get; set; }
    public string? Abstract { get; set; }
    public string? Doi { get; set; }
}
