using WWN.Application.DTOs;
using WWN.Domain.Entities;
using WWN.Domain.Interfaces;

namespace WWN.Application.Services;

public class FocusDefinitionService
{
    private readonly IFocusDefinitionRepository _repo;

    public FocusDefinitionService(IFocusDefinitionRepository repo)
    {
        _repo = repo;
    }

    public async Task<IReadOnlyList<FocusDefinitionDto>> ListAsync(CancellationToken ct = default)
    {
        var foci = await _repo.GetAllAsync(ct);
        return foci.Select(MapToDto).ToList();
    }

    public async Task<FocusDefinitionDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var fd = await _repo.GetByIdAsync(id, ct);
        return fd is null ? null : MapToDto(fd);
    }

    public async Task<FocusDefinitionDto> CreateAsync(CreateFocusDefinitionRequest req, CancellationToken ct = default)
    {
        var fd = new FocusDefinition(
            req.Name,
            req.Level1Description,
            req.Level2Description,
            req.Description,
            req.CanTakeMultipleTimes);
        await _repo.AddAsync(fd, ct);
        return MapToDto(fd);
    }

    public async Task<FocusDefinitionDto?> UpdateAsync(Guid id, UpdateFocusDefinitionRequest req, CancellationToken ct = default)
    {
        var fd = await _repo.GetByIdAsync(id, ct);
        if (fd is null) return null;

        fd.Update(req.Name, req.Description, req.Level1Description, req.Level2Description, req.CanTakeMultipleTimes);
        await _repo.UpdateAsync(fd, ct);
        return MapToDto(fd);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        => await _repo.DeleteAsync(id, ct);

    internal static FocusDefinitionDto MapToDto(FocusDefinition fd) => new()
    {
        Id = fd.Id,
        Name = fd.Name,
        Description = fd.Description,
        Level1Description = fd.Level1Description,
        Level2Description = fd.Level2Description,
        HasLevel2 = fd.HasLevel2,
        CanTakeMultipleTimes = fd.CanTakeMultipleTimes,
    };
}
