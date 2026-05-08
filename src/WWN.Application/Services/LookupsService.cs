using WWN.Application.DTOs;
using WWN.Domain.Lookups;

namespace WWN.Application.Services;

/// <summary>
/// Aggregates all lookup catalogs into a single DTO returned by <c>GET /api/lookups</c>.
/// The aggregated payload never changes within a process lifetime (catalogs are static),
/// so we cache it eagerly and hand out the same reference to every caller.
/// </summary>
public class LookupsService
{
    private static readonly LookupsDto Cached = Build();
    private static readonly string CachedETag = ComputeETag(Cached);

    public LookupsDto GetAll() => Cached;

    public string ETag => CachedETag;

    private static LookupsDto Build() => new()
    {
        EffortCommitment = EffortCommitmentCatalog.All.Select(Map).ToArray()
    };

    private static LookupValueDto Map(LookupValue v) => new()
    {
        Id = v.Id,
        Code = v.Code,
        DisplayName = v.DisplayName,
        Description = v.Description,
        SortOrder = v.SortOrder
    };

    private static string ComputeETag(LookupsDto dto)
    {
        // The payload is process-static; a hash of its content gives a stable ETag
        // that changes only when the catalog content changes (i.e. across deployments).
        var payload = string.Join("|",
            dto.EffortCommitment.Select(v => $"{v.Id}:{v.Code}:{v.DisplayName}:{v.Description}:{v.SortOrder}"));
        var bytes = System.Text.Encoding.UTF8.GetBytes(payload);
        var hash = System.Security.Cryptography.SHA256.HashData(bytes);
        return $"\"{Convert.ToHexString(hash)[..16]}\"";
    }
}
