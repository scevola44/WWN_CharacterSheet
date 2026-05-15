using WWN.Domain.Entities;
using WWN.Domain.Interfaces;

namespace WWN.Application.Services;

/// <summary>
/// Seeds the Arts table from markdown files in DefaultData/Arts.
/// Markdown files follow Obsidian.md structure with YAML frontmatter containing Art metadata.
/// </summary>
public class ArtDefinitionSeeder(IArtRepository artRepository, IArtSourceRepository artSourceRepository)
{
    private const string ArtSourceCode = "HighMagic";
    private const string ArtSourceDisplayName = "High Magic";
    private const string DefaultDataArtsPath = "DefaultData/Arts";

    public async Task SeedIfEmptyAsync(CancellationToken cancellationToken = default)
    {
        if (await artRepository.AnyGlobalAsync(cancellationToken)) return;
        await SeedCoreAsync(cancellationToken);
    }

    public async Task ForceReseedAsync(CancellationToken cancellationToken = default)
    {
        foreach (var existing in await artRepository.GetGlobalAsync(cancellationToken))
            await artRepository.DeleteAsync(existing.Id, cancellationToken);
        await SeedCoreAsync(cancellationToken);
    }

    private async Task SeedCoreAsync(CancellationToken cancellationToken)
    {
        var sourceId = await EnsureArtSourceAsync(cancellationToken);
        await SeedArtsFromMarkdownAsync(sourceId, cancellationToken);
    }

    private async Task<int> EnsureArtSourceAsync(CancellationToken cancellationToken)
    {
        var existingSource = await artSourceRepository.GetAllAsync(cancellationToken);
        var highMagicSource = existingSource.FirstOrDefault(s => s.Code == ArtSourceCode);

        if (highMagicSource != null)
            return highMagicSource.Id;

        var newSource = new ArtSource(ArtSourceCode, ArtSourceDisplayName, null, 1);
        await artSourceRepository.AddAsync(newSource, cancellationToken);

        var allSources = await artSourceRepository.GetAllAsync(cancellationToken);
        return allSources.First(s => s.Code == ArtSourceCode).Id;
    }

    private async Task SeedArtsFromMarkdownAsync(int sourceId, CancellationToken cancellationToken)
    {
        var artsDir = Path.Combine(Directory.GetCurrentDirectory(), DefaultDataArtsPath);
        if (!Directory.Exists(artsDir))
        {
            Console.WriteLine($"Arts directory not found: {artsDir}");
            return;
        }

        var markdownFiles = Directory.GetFiles(artsDir, "*.md")
            .OrderBy(f => Path.GetFileName(f))
            .ToList();

        foreach (var filePath in markdownFiles)
        {
            try
            {
                var art = ArtMarkdownParser.ParseArtFile(filePath, sourceId);
                if (art != null)
                {
                    await artRepository.AddAsync(art, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding art from {Path.GetFileName(filePath)}: {ex.Message}");
            }
        }
    }
}
