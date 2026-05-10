using WWN.Domain.Entities;
using WWN.Domain.Interfaces;

namespace WWN.Application.Services;

/// <summary>
/// Seeds the Spells table with spells from markdown files in DefaultData/Spells/
/// (High Magic tradition from Worlds Without Number rulebook).
/// </summary>
public class SpellDefinitionSeeder(ISpellRepository spellRepository)
{
    private const string DefaultSpellsPath = "DefaultData/Spells";

    public async Task SeedIfEmptyAsync(CancellationToken cancellationToken = default)
    {
        if (await spellRepository.AnyAsync(cancellationToken)) return;
        await SeedCoreAsync(cancellationToken);
    }

    public async Task ForceReseedAsync(CancellationToken cancellationToken = default)
    {
        foreach (var existing in await spellRepository.GetAllAsync(cancellationToken))
            await spellRepository.DeleteAsync(existing.Id, cancellationToken);
        await SeedCoreAsync(cancellationToken);
    }

    private async Task SeedCoreAsync(CancellationToken cancellationToken)
    {
        foreach (var spell in CreateSpellsFromMarkdown())
            if (spell != null)
                await spellRepository.AddAsync(spell, cancellationToken);
    }

    private static IEnumerable<Spell?> CreateSpellsFromMarkdown()
    {
        if (!Directory.Exists(DefaultSpellsPath))
        {
            Console.WriteLine($"Warning: DefaultData/Spells directory not found at {DefaultSpellsPath}");
            return Enumerable.Empty<Spell?>();
        }

        var markdownFiles = Directory.GetFiles(DefaultSpellsPath, "*.md");
        return markdownFiles
            .OrderBy(f => Path.GetFileNameWithoutExtension(f))
            .Select(SpellMarkdownParser.ParseSpellFile);
    }

}
