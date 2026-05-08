using WWN.Application.DTOs;
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
            ToEffortCommitment(request.EffortCost),
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
            ToEffortCommitment(request.EffortCost),
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
            EffortCost = (int)art.EffortCost,
            Source = art.Source
        };
    }

    private static EffortCommitment ToEffortCommitment(int value)
    {
        var enumValue = (EffortCommitment)value;
        if (!Enum.IsDefined(enumValue))
            throw new ArgumentException(
                $"'{value}' is not a valid EffortCommitment id.",
                nameof(value));
        return enumValue;
    }
}
