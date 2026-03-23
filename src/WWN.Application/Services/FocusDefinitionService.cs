using WWN.Application.DTOs;
using WWN.Domain.Entities;
using WWN.Domain.Interfaces;

namespace WWN.Application.Services;

public class FocusDefinitionService(IFocusDefinitionRepository focusDefinitionRepository)
{
    public async Task<IReadOnlyList<FocusDefinitionDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var foci = await focusDefinitionRepository.GetAllAsync(cancellationToken);
        return foci.Select(MapToDto).ToList();
    }

    public async Task<FocusDefinitionDto?> GetAsync(
        Guid focusId, 
        CancellationToken cancellationToken = default)
    {
        var focusDefinition = await focusDefinitionRepository.GetByIdAsync(focusId, cancellationToken);
        return focusDefinition is null ? null : MapToDto(focusDefinition);
    }

    public async Task<FocusDefinitionDto> CreateAsync(
        CreateFocusDefinitionRequest request, 
        CancellationToken cancellationToken = default)
    {
        var focusDefinition = new FocusDefinition(
            request.Name,
            request.Level1Description,
            request.Level2Description,
            request.Description,
            request.CanTakeMultipleTimes);
        await focusDefinitionRepository.AddAsync(focusDefinition, cancellationToken);
        return MapToDto(focusDefinition);
    }

    public async Task<FocusDefinitionDto?> UpdateAsync(
        Guid focusId, 
        UpdateFocusDefinitionRequest request, 
        CancellationToken cancellationToken = default)
    {
        var focusDefinition = await focusDefinitionRepository.GetByIdAsync(focusId, cancellationToken);
        if (focusDefinition is null) return null;

        focusDefinition.Update(request.Name, request.Description, request.Level1Description, request.Level2Description, request.CanTakeMultipleTimes);
        await focusDefinitionRepository.UpdateAsync(focusDefinition, cancellationToken);
        return MapToDto(focusDefinition);
    }

    public async Task DeleteAsync(Guid focusId, CancellationToken cancellationToken = default)
        => await focusDefinitionRepository.DeleteAsync(focusId, cancellationToken);

    private static FocusDefinitionDto MapToDto(FocusDefinition fd) => new()
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
