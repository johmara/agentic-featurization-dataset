using System.Text;
using System.Text.RegularExpressions;
using ReferenceManager.Models;

namespace ReferenceManager.Services;

public static class BibtexSerializer
{
    public static string Serialize(IEnumerable<Paper> papers)
    {
        var sb = new StringBuilder();
        var usedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var paper in papers)
        {
            var type = paper.Journal is not null ? "article"
                : paper.Booktitle is not null ? "inproceedings"
                : "misc";

            var key = GenerateKey(paper, usedKeys);
            usedKeys.Add(key);

            sb.AppendLine($"@{type}{{{key},");
            sb.AppendLine($"  title = {{{Escape(paper.Title)}}},");

            if (paper.Authors.Count > 0)
                sb.AppendLine($"  author = {{{string.Join(" and ", paper.Authors.Select(a => Escape(a.Name)))}}},");

            if (paper.Year > 0)
                sb.AppendLine($"  year = {{{paper.Year}}},");

            if (paper.Journal is not null)
                sb.AppendLine($"  journal = {{{Escape(paper.Journal)}}},");

            if (paper.Booktitle is not null)
                sb.AppendLine($"  booktitle = {{{Escape(paper.Booktitle)}}},");

            if (paper.Doi is not null)
                sb.AppendLine($"  doi = {{{paper.Doi}}},");

            if (paper.Abstract is not null)
                sb.AppendLine($"  abstract = {{{Escape(paper.Abstract)}}},");

            sb.AppendLine("}");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static string GenerateKey(Paper paper, HashSet<string> used)
    {
        var lastName = paper.Authors.Count > 0
            ? ExtractLastName(paper.Authors[0].Name)
            : "unknown";
        var year = paper.Year > 0 ? paper.Year.ToString() : "0000";
        var baseKey = $"{lastName}{year}";

        if (!used.Contains(baseKey)) return baseKey;

        for (var c = 'a'; c <= 'z'; c++)
        {
            var candidate = baseKey + c;
            if (!used.Contains(candidate)) return candidate;
        }
        return baseKey + Guid.NewGuid().ToString("N")[..4];
    }

    private static string ExtractLastName(string name)
    {
        var clean = Regex.Replace(name, @"[^a-zA-Z ,\-]", "");
        var parts = clean.Split(',', 2, StringSplitOptions.TrimEntries);
        var lastName = parts.Length >= 2
            ? parts[0]
            : clean.Split(' ', StringSplitOptions.RemoveEmptyEntries).LastOrDefault() ?? "unknown";
        return Regex.Replace(lastName.ToLowerInvariant(), @"[^a-z]", "");
    }

    private static string Escape(string value) =>
        value.Replace("{", "\\{").Replace("}", "\\}");
}
