namespace ReferenceManager.Models;

public class Author
{
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public List<Affiliation> Affiliations { get; set; } = [];
}
