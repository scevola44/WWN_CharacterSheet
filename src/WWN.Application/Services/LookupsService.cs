using WWN.Application.DTOs;
using WWN.Domain.Interfaces;
using WWN.Domain.Lookups;

namespace WWN.Application.Services;

public class LookupsService(IArtSourceRepository artSourceRepository)
{
    public async Task<LookupsDto> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var artSources = await artSourceRepository.GetAllAsync(cancellationToken);
        return new LookupsDto
        {
            EffortCommitment = EffortCommitmentCatalog.All.Select(MapLookupValue).ToArray(),
            ArtSources = artSources.Select(s => new LookupValueDto
            {
                Id = s.Id,
                Code = s.Code,
                DisplayName = s.DisplayName,
                Description = s.Description,
                SortOrder = s.SortOrder
            }).ToArray(),
            SkillNames = SkillNameCatalog.All.Select(MapLookupValue).ToArray(),
            AttributeNames = AttributeNameCatalog.All.Select(MapLookupValue).ToArray(),
            ItemSlotTypes = ItemSlotTypeCatalog.All.Select(MapLookupValue).ToArray(),
            SaveTypes = SaveTypeCatalog.All.Select(MapLookupValue).ToArray(),
            CharacterClasses = CharacterClassCatalog.All.Select(MapLookupValue).ToArray(),
            PartialClasses = PartialClassCatalog.All.Select(MapLookupValue).ToArray(),
            FocusEffectTypes = FocusEffectTypeCatalog.All.Select(MapLookupValue).ToArray(),
            WeaponTags = WeaponTagCatalog.All.Select(MapLookupValue).ToArray(),
        };
    }

    public static string ComputeETag(LookupsDto dto)
    {
        var allValues = dto.EffortCommitment
            .Concat(dto.ArtSources)
            .Concat(dto.SkillNames)
            .Concat(dto.AttributeNames)
            .Concat(dto.ItemSlotTypes)
            .Concat(dto.SaveTypes)
            .Concat(dto.CharacterClasses)
            .Concat(dto.PartialClasses)
            .Concat(dto.FocusEffectTypes)
            .Concat(dto.WeaponTags);
        var payload = string.Join("|", allValues.Select(v => $"{v.Id}:{v.Code}:{v.DisplayName}:{v.Description}:{v.SortOrder}:{v.Abbreviation}"));
        var bytes = System.Text.Encoding.UTF8.GetBytes(payload);
        var hash = System.Security.Cryptography.SHA256.HashData(bytes);
        return $"\"{Convert.ToHexString(hash)[..16]}\"";
    }

    private static LookupValueDto MapLookupValue(LookupValue v) => new()
    {
        Id = v.Id,
        Code = v.Code,
        DisplayName = v.DisplayName,
        Description = v.Description,
        SortOrder = v.SortOrder,
        Abbreviation = v.Abbreviation
    };
}
