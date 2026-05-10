using System.Text.RegularExpressions;
using WWN.Domain.Entities;
using WWN.Domain.Enums;

namespace WWN.Application.Services;

public class ArtMarkdownParser
{
    private const string FrontmatterStart = "---";
    private const string FrontmatterEnd = "---";

    public static Art? ParseArtFile(string filePath, int sourceId)
    {
        try
        {
            var content = File.ReadAllText(filePath);
            return ParseArtContent(content, sourceId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing file {filePath}: {ex.Message}");
            return null;
        }
    }

    public static Art? ParseArtContent(string content, int sourceId)
    {
        var (frontmatter, body) = ExtractFrontmatter(content);
        if (frontmatter == null) return null;

        var name = ExtractName(body);
        var description = ExtractDescription(body);
        var summary = ExtractYamlValue(frontmatter, "description");
        var effort = ParseEffort(ExtractYamlValue(frontmatter, "effort"));

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description))
            return null;

        return new Art(
            name: name,
            description: description,
            minLevel: 1,
            sourceId: sourceId,
            effortCost: effort,
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

    private static EffortCommitment ParseEffort(string? effortString)
    {
        if (string.IsNullOrWhiteSpace(effortString))
            return EffortCommitment.None;

        return effortString.ToLowerInvariant() switch
        {
            "scene" => EffortCommitment.Scene,
            "day" => EffortCommitment.Day,
            "sustained" => EffortCommitment.Sustained,
            "instant" => EffortCommitment.Sustained, // Arts that use "Instant" action but sustain effort
            "no effort" => EffortCommitment.None,
            _ => EffortCommitment.None
        };
    }
}
