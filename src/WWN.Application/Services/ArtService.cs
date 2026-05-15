using WWN.Application.DTOs;
using WWN.Domain.Entities;
using WWN.Domain.Enums;
using WWN.Domain.Interfaces;

namespace WWN.Application.Services;

public class ArtService(IArtRepository artRepository)
{
    public async Task<IReadOnlyList<ArtDto>> ListArtsAsync(
        string? userId = null,
        CancellationToken cancellationToken = default)
    {
        var arts = await artRepository.GetVisibleToUserAsync(userId, cancellationToken);
        return arts.Select(MapToDto).ToList();
    }

    public async Task<ArtDto?> GetArtAsync(
        Guid artId,
        string? userId = null,
        CancellationToken cancellationToken = default)
    {
        var art = await artRepository.GetByIdAsync(artId, cancellationToken);
        if (art is null) return null;
        if (art.OwnerId is not null && art.OwnerId != userId) return null;
        return MapToDto(art);
    }

    public async Task<ArtDto> CreateArtAsync(
        CreateArtRequest request,
        string? ownerId = null,
        CancellationToken cancellationToken = default)
    {
        var art = new Art(
            request.Name,
            request.Description,
            request.MinLevel,
            request.SourceId,
            ToEffortCommitment(request.EffortCost),
            request.Summary,
            ownerId);
        await artRepository.AddAsync(art, cancellationToken);
        return MapToDto(art);
    }

    public async Task<ArtDto?> UpdateArtAsync(
        Guid artId,
        UpdateArtRequest request,
        string? userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var art = await artRepository.GetByIdAsync(artId, cancellationToken);
        if (art is null) return null;
        EnsureCanMutate(art.OwnerId, userId, isAdmin);

        art.Update(
            request.Name,
            request.Description,
            request.MinLevel,
            request.SourceId,
            ToEffortCommitment(request.EffortCost),
            request.Summary);
        await artRepository.UpdateAsync(art, cancellationToken);
        return MapToDto(art);
    }

    public async Task DeleteArtAsync(
        Guid artId,
        string? userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var art = await artRepository.GetByIdAsync(artId, cancellationToken);
        if (art is null) return;
        EnsureCanMutate(art.OwnerId, userId, isAdmin);
        await artRepository.DeleteAsync(artId, cancellationToken);
    }

    private static void EnsureCanMutate(string? ownerId, string? userId, bool isAdmin)
    {
        if (isAdmin) return;
        if (ownerId is null)
            throw new UnauthorizedAccessException("Only an admin can modify built-in arts.");
        if (ownerId != userId)
            throw new UnauthorizedAccessException("You can only modify arts you created.");
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
            SourceId = art.SourceId,
            IsCustom = art.OwnerId is not null
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
