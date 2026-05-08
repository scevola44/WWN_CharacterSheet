using WWN.Application.DTOs;
using WWN.Domain.Entities;
using WWN.Domain.Interfaces;

namespace WWN.Application.Services;

public class ArtSourceService(IArtSourceRepository artSourceRepository, IArtRepository artRepository)
{
    public async Task<IReadOnlyList<LookupValueDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var sources = await artSourceRepository.GetAllAsync(cancellationToken);
        return sources.Select(MapToDto).ToList();
    }

    public async Task<LookupValueDto?> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var source = await artSourceRepository.GetByIdAsync(id, cancellationToken);
        return source is null ? null : MapToDto(source);
    }

    public async Task<LookupValueDto> CreateAsync(CreateArtSourceRequest request, CancellationToken cancellationToken = default)
    {
        if (await artSourceRepository.CodeExistsAsync(request.Code, cancellationToken: cancellationToken))
            throw new InvalidOperationException($"Code '{request.Code}' is already in use.");

        var source = new ArtSource(request.Code, request.DisplayName, request.Description, request.SortOrder);
        await artSourceRepository.AddAsync(source, cancellationToken);
        return MapToDto(source);
    }

    public async Task<LookupValueDto?> UpdateAsync(int id, UpdateArtSourceRequest request, CancellationToken cancellationToken = default)
    {
        var source = await artSourceRepository.GetByIdAsync(id, cancellationToken);
        if (source is null) return null;

        source.Update(request.DisplayName, request.Description, request.SortOrder);
        await artSourceRepository.UpdateAsync(source, cancellationToken);
        return MapToDto(source);
    }

    // Returns null = not found, false = in use (409), true = deleted.
    public async Task<bool?> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var source = await artSourceRepository.GetByIdAsync(id, cancellationToken);
        if (source is null) return null;

        if (await artRepository.AnyWithSourceIdAsync(id, cancellationToken))
            return false;

        await artSourceRepository.DeleteAsync(id, cancellationToken);
        return true;
    }

    private static LookupValueDto MapToDto(ArtSource s) => new()
    {
        Id = s.Id,
        Code = s.Code,
        DisplayName = s.DisplayName,
        Description = s.Description,
        SortOrder = s.SortOrder
    };
}
