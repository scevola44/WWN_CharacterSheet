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
            }).ToArray()
        };
    }

    public static string ComputeETag(LookupsDto dto)
    {
        var payload = string.Join("|",
            dto.EffortCommitment
                .Select(v => $"{v.Id}:{v.Code}:{v.DisplayName}:{v.Description}:{v.SortOrder}")
                .Concat(dto.ArtSources
                    .Select(v => $"{v.Id}:{v.Code}:{v.DisplayName}:{v.Description}:{v.SortOrder}")));
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
        SortOrder = v.SortOrder
    };
}
