using WWN.Application.DTOs;
using WWN.Application.Helpers;
using WWN.Domain.Entities;
using WWN.Domain.Enums;
using WWN.Domain.Interfaces;

namespace WWN.Application.Services;

public class ArtService(IArtRepository artRepository)
{
    public async Task<IReadOnlyList<ArtDto>> ListArtsAsync(CancellationToken cancellationToken = default)
    {
        var arts = await artRepository.GetAllAsync(cancellationToken);
        return arts.Select(MapToDto).ToList();
    }

    public async Task<ArtDto?> GetArtAsync(Guid artId, CancellationToken cancellationToken = default)
    {
        var art = await artRepository.GetByIdAsync(artId, cancellationToken);
        return art is null ? null : MapToDto(art);
    }

    public async Task<ArtDto> CreateArtAsync(
        CreateArtRequest request,
        CancellationToken cancellationToken = default)
    {
        var art = new Art(
            request.Name,
            request.Description,
            request.MinLevel,
            request.Source,
            ParseEffortCost(request.EffortCost),
            request.Summary);
        await artRepository.AddAsync(art, cancellationToken);
        return MapToDto(art);
    }

    public async Task<ArtDto?> UpdateArtAsync(
        Guid artId,
        UpdateArtRequest request,
        CancellationToken cancellationToken = default)
    {
        var art = await artRepository.GetByIdAsync(artId, cancellationToken);
        if (art is null) return null;

        art.Update(
            request.Name,
            request.Description,
            request.MinLevel,
            request.Source,
            ParseEffortCost(request.EffortCost),
            request.Summary);
        await artRepository.UpdateAsync(art, cancellationToken);
        return MapToDto(art);
    }

    public async Task DeleteArtAsync(Guid artId, CancellationToken cancellationToken = default)
    {
        await artRepository.DeleteAsync(artId, cancellationToken);
    }

    internal static ArtDto MapToDto(Art art)
    {
        return new ArtDto
        {
            Id = art.Id,
            Name = art.Name,
            Description = art.Description,
            Summary = art.Summary,
            MinLevel = art.MinLevel,
            EffortCost = art.EffortCost?.ToString(),
            Source = art.Source
        };
    }

    private static EffortCommitment? ParseEffortCost(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        return EnumParser.Parse<EffortCommitment>(value, nameof(value));
    }
}
