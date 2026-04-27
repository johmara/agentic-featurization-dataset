namespace ReferenceManager.Models;

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<Paper> Papers { get; set; } = [];
}
