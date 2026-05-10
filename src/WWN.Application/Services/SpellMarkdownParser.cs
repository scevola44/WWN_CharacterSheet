using System.Text.RegularExpressions;
using WWN.Domain.Entities;

namespace WWN.Application.Services;

public class SpellMarkdownParser
{
    private const string FrontmatterStart = "---";
    private const string FrontmatterEnd = "---";

    public static Spell? ParseSpellFile(string filePath)
    {
        try
        {
            var content = File.ReadAllText(filePath);
            return ParseSpellContent(content);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing spell file {filePath}: {ex.Message}");
            return null;
        }
    }

    public static Spell? ParseSpellContent(string content)
    {
        var (frontmatter, body) = ExtractFrontmatter(content);
        if (frontmatter == null) return null;

        var name = ExtractName(body);
        var description = ExtractDescription(body);
        var summary = ExtractYamlValue(frontmatter, "summary");
        var levelString = ExtractYamlValue(frontmatter, "level");

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description))
            return null;

        if (!int.TryParse(levelString, out var spellLevel) || spellLevel < 1 || spellLevel > 6)
            return null;

        return new Spell(
            name: name,
            spellLevel: spellLevel,
            description: description,
            summary: summary
        );
    }

    private static (string? frontmatter, string body) ExtractFrontmatter(string content)
    {
        var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        if (lines.Length < 3 || lines[0].Trim() != FrontmatterStart)
            return (null, content);

        var endIndex = -1;
        for (var i = 1; i < lines.Length; i++)
        {
            if (lines[i].Trim() == FrontmatterEnd)
            {
                endIndex = i;
                break;
            }
        }

        if (endIndex <= 0)
            return (null, content);

        var frontmatterLines = lines.Skip(1).Take(endIndex - 1).ToArray();
        var bodyLines = lines.Skip(endIndex + 1).ToArray();

        return (string.Join("\n", frontmatterLines), string.Join("\n", bodyLines));
    }

    private static string? ExtractYamlValue(string frontmatter, string key)
    {
        var pattern = $@"^{Regex.Escape(key)}\s*:\s*[""']?([^""'\n]*)[""']?";
        var match = Regex.Match(frontmatter, pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value.Trim() : null;
    }

    private static string ExtractName(string body)
    {
        var match = Regex.Match(body, @"^#\s+(.+)$", RegexOptions.Multiline);
        return match.Success ? match.Groups[1].Value.Trim() : string.Empty;
    }

    private static string ExtractDescription(string body)
    {
        // Remove the heading and extract everything after it
        var withoutHeading = Regex.Replace(body, @"^#\s+.+\n", "", RegexOptions.Multiline);
        return withoutHeading.Trim();
    }
}
