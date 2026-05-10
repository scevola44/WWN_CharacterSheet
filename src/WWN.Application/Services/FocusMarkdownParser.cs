using System.Text.RegularExpressions;
using WWN.Domain.Entities;

namespace WWN.Application.Services;

public class FocusMarkdownParser
{
    public static FocusDefinition? ParseFocusFile(string filePath)
    {
        try
        {
            var content = File.ReadAllText(filePath);
            return ParseFocusContent(content);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing focus file {filePath}: {ex.Message}");
            return null;
        }
    }

    public static FocusDefinition? ParseFocusContent(string content)
    {
        var name = ExtractName(content);
        if (string.IsNullOrWhiteSpace(name))
            return null;

        var description = ExtractDescription(content);
        var level1Description = ExtractLevelDescription(content, 1);
        var level2Description = ExtractLevelDescription(content, 2);

        if (string.IsNullOrWhiteSpace(level1Description))
            return null;

        return new FocusDefinition(
            name: name.Trim(),
            description: string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            level1Description: level1Description.Trim(),
            level2Description: string.IsNullOrWhiteSpace(level2Description) ? null : level2Description.Trim()
        );
    }

    private static string? ExtractName(string content)
    {
        var match = Regex.Match(content, @"^#\s+(.+)$", RegexOptions.Multiline);
        return match.Success ? match.Groups[1].Value : null;
    }

    private static string? ExtractDescription(string content)
    {
        var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        var nameIndex = -1;
        for (var i = 0; i < lines.Length; i++)
        {
            if (Regex.IsMatch(lines[i], @"^#\s+"))
            {
                nameIndex = i;
                break;
            }
        }

        if (nameIndex < 0)
            return null;

        var levelIndex = -1;
        for (var i = nameIndex + 1; i < lines.Length; i++)
        {
            if (Regex.IsMatch(lines[i], @"^##\s+"))
            {
                levelIndex = i;
                break;
            }
        }

        if (levelIndex < 0)
            return null;

        var descriptionLines = lines
            .Skip(nameIndex + 1)
            .Take(levelIndex - nameIndex - 1)
            .ToArray();

        var description = string.Join("\n", descriptionLines).Trim();
        return string.IsNullOrWhiteSpace(description) ? null : description;
    }

    private static string? ExtractLevelDescription(string content, int level)
    {
        var pattern = $@"^##\s+Level\s+{level}\s*$";
        var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        var levelIndex = -1;
        for (var i = 0; i < lines.Length; i++)
        {
            if (Regex.IsMatch(lines[i], pattern, RegexOptions.IgnoreCase))
            {
                levelIndex = i;
                break;
            }
        }

        if (levelIndex < 0)
            return null;

        var nextHeadingIndex = -1;
        for (var i = levelIndex + 1; i < lines.Length; i++)
        {
            if (Regex.IsMatch(lines[i], @"^##\s+"))
            {
                nextHeadingIndex = i;
                break;
            }
        }

        var endIndex = nextHeadingIndex >= 0 ? nextHeadingIndex : lines.Length;
        var descriptionLines = lines
            .Skip(levelIndex + 1)
            .Take(endIndex - levelIndex - 1)
            .ToArray();

        var description = string.Join("\n", descriptionLines).Trim();
        return string.IsNullOrWhiteSpace(description) ? null : description;
    }
}
