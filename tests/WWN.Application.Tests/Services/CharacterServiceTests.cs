using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NSubstitute;
using WWN.Application.DTOs;
using WWN.Application.Services;
using WWN.Domain.Aggregates;
using WWN.Domain.Entities;
using WWN.Domain.Enums;
using WWN.Domain.Interfaces;
using WWN.Domain.ValueObjects;

namespace WWN.Application.Tests.Services;

public class CharacterServiceTests
{
    private const string UserId = "user-1";

    private readonly ICharacterRepository _characterRepository;
    private readonly CharacterIdentityService _identity;
    private readonly CharacterFocusService _focusService;
    private readonly CharacterInventoryService _inventory;

    public CharacterServiceTests()
    {
        _characterRepository = Substitute.For<ICharacterRepository>();
        var focusDefinitionRepository = Substitute.For<IFocusDefinitionRepository>();
        var classAbilityRepository = Substitute.For<IClassAbilityRepository>();

        focusDefinitionRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<FocusDefinition>>(Array.Empty<FocusDefinition>()));
        classAbilityRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<ClassAbilityDefinition>>(Array.Empty<ClassAbilityDefinition>()));

        _characterRepository
            .ExecuteInTransactionAsync(
                Arg.Any<Func<Task<CharacterDetailDto>>>(),
                Arg.Any<CancellationToken>())
            .Returns(ci => ci.ArgAt<Func<Task<CharacterDetailDto>>>(0)());

        var cache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
        var mapper = new CharacterDetailMapper(focusDefinitionRepository, classAbilityRepository, cache);
        _identity = new CharacterIdentityService(_characterRepository, mapper);
        _focusService = new CharacterFocusService(_characterRepository, mapper);
        _inventory = new CharacterInventoryService(_characterRepository, mapper);
    }

    private static Dictionary<string, int> DefaultAttributeStrings() => new()
    {
        [nameof(AttributeName.Strength)] = 10,
        [nameof(AttributeName.Dexterity)] = 10,
        [nameof(AttributeName.Constitution)] = 10,
        [nameof(AttributeName.Intelligence)] = 10,
        [nameof(AttributeName.Wisdom)] = 10,
        [nameof(AttributeName.Charisma)] = 10
    };

    private static CreateCharacterRequest WarriorRequest() => new()
    {
        Name = "Test",
        Class = nameof(CharacterClass.Warrior),
        Attributes = DefaultAttributeStrings()
    };

    private static Character NewWarrior(string name = "Test")
    {
        var scores = Enum.GetValues<AttributeName>().ToDictionary(a => a, _ => 10);
        return Character.Create(name, CharacterClass.Warrior, scores, UserId);
    }

    [Fact]
    public async Task CreateCharacterAsync_HappyPath_AddsToRepositoryAndReturnsId()
    {
        var id = await _identity.CreateCharacterAsync(WarriorRequest(), UserId);

        id.Should().NotBeEmpty();
        await _characterRepository.Received(1).AddAsync(
            Arg.Is<Character>(c => c.Name == "Test"
                && c.Class == CharacterClass.Warrior
                && c.UserId == UserId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCharacterAsync_AdventurerWithoutPartials_Throws()
    {
        var request = WarriorRequest() with { Class = nameof(CharacterClass.Adventurer) };

        var act = () => _identity.CreateCharacterAsync(request, UserId);

        await act.Should().ThrowAsync<ArgumentException>();
        await _characterRepository.DidNotReceive().AddAsync(
            Arg.Any<Character>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCharacterAsync_AdventurerWithDuplicatePartials_Throws()
    {
        var request = WarriorRequest() with
        {
            Class = nameof(CharacterClass.Adventurer),
            PartialClassA = nameof(PartialClass.PartialMage),
            PartialClassB = nameof(PartialClass.PartialMage)
        };

        var act = () => _identity.CreateCharacterAsync(request, UserId);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*distinct*");
    }

    [Fact]
    public async Task CreateCharacterAsync_InvalidClassEnum_Throws()
    {
        var request = WarriorRequest() with { Class = "Bard" };

        var act = () => _identity.CreateCharacterAsync(request, UserId);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateAttributeAsync_CharacterNotFound_Throws()
    {
        _characterRepository.GetByIdAsync(Arg.Any<Guid>(), UserId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Character?>(null));

        var act = () => _identity.UpdateAttributeAsync(
            Guid.NewGuid(), UserId, nameof(AttributeName.Strength), 14);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task UpdateAttributeAsync_HappyPath_UpdatesScoreAndPersists()
    {
        var character = NewWarrior();
        _characterRepository.GetByIdAsync(character.Id, UserId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Character?>(character));

        var dto = await _identity.UpdateAttributeAsync(
            character.Id, UserId, nameof(AttributeName.Strength), 16);

        character.GetAttribute(AttributeName.Strength).Score.Value.Should().Be(16);
        await _characterRepository.Received(1).UpdateAsync(character, Arg.Any<CancellationToken>());
        dto.Attributes.Single(a => a.Name == nameof(AttributeName.Strength)).Score.Should().Be(16);
    }

    [Fact]
    public async Task UpdateSkillAsync_HappyPath_SetsRankAndPersists()
    {
        var character = NewWarrior();
        _characterRepository.GetByIdAsync(character.Id, UserId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Character?>(character));

        await _identity.UpdateSkillAsync(character.Id, UserId, nameof(SkillName.Stab), 2);

        character.GetSkill(SkillName.Stab).Rank.Level.Should().Be(2);
        await _characterRepository.Received(1).UpdateAsync(character, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddCustomSkillAsync_HappyPath_AddsSkill()
    {
        var character = NewWarrior();
        _characterRepository.GetByIdAsync(character.Id, UserId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Character?>(character));

        await _identity.AddCustomSkillAsync(character.Id, UserId, "Alchemy", 0);

        character.Skills.Should().Contain(s => s.CustomName == "Alchemy");
        await _characterRepository.Received(1).UpdateAsync(character, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SetHpAsync_HappyPath_UpdatesHpAndPersists()
    {
        var character = NewWarrior();
        _characterRepository.GetByIdAsync(character.Id, UserId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Character?>(character));

        await _identity.SetHpAsync(character.Id, UserId, 12, 8);

        character.MaxHitPoints.Should().Be(12);
        character.CurrentHitPoints.Should().Be(8);
    }

    [Fact]
    public async Task LevelUpAsync_HappyPath_IncrementsLevelAndAddsHp()
    {
        var character = NewWarrior();
        var startingLevel = character.Level;
        var startingMax = character.MaxHitPoints;
        _characterRepository.GetByIdAsync(character.Id, UserId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Character?>(character));

        await _identity.LevelUpAsync(character.Id, UserId, hpGain: 5);

        character.Level.Should().Be(startingLevel + 1);
        character.MaxHitPoints.Should().Be(startingMax + 5);
        await _characterRepository.Received(1).UpdateAsync(character, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddFocusAsync_HappyPath_AppendsFocus()
    {
        var character = NewWarrior();
        _characterRepository.GetByIdAsync(character.Id, UserId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Character?>(character));

        var request = new AddFocusRequest
        {
            Name = "Alert",
            Level = 1,
            Effects = new List<FocusEffectDto>
            {
                new() { Type = nameof(FocusEffectType.Initiative), NumericValue = 1 }
            }
        };

        await _focusService.AddFocusAsync(character.Id, UserId, request);

        character.Foci.Should().ContainSingle(f => f.Name == "Alert" && f.Level == 1);
    }

    [Fact]
    public async Task RemoveFocusAsync_HappyPath_RemovesFocus()
    {
        var character = NewWarrior();
        var focus = new Focus("Alert", 1, Array.Empty<FocusEffect>());
        character.AddFocus(focus);
        _characterRepository.GetByIdAsync(character.Id, UserId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Character?>(character));

        await _focusService.RemoveFocusAsync(character.Id, UserId, focus.Id);

        character.Foci.Should().BeEmpty();
        await _characterRepository.Received(1).UpdateAsync(character, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddItemAsync_HappyPath_AddsItem()
    {
        var character = NewWarrior();
        _characterRepository.GetByIdAsync(character.Id, UserId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Character?>(character));

        var request = new AddItemRequest
        {
            Name = "Rope",
            Encumbrance = 1,
            ItemType = "Item",
            SlotType = nameof(ItemSlotType.Stowed)
        };

        await _inventory.AddItemAsync(character.Id, UserId, request);

        character.Inventory.Should().ContainSingle(i => i.Name == "Rope");
    }

    [Fact]
    public async Task RemoveItemAsync_HappyPath_RemovesItem()
    {
        var character = NewWarrior();
        var item = new Item("Rope", 1);
        character.AddItem(item);
        _characterRepository.GetByIdAsync(character.Id, UserId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Character?>(character));

        await _inventory.RemoveItemAsync(character.Id, UserId, item.Id);

        character.Inventory.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteCharacterAsync_DelegatesToRepository()
    {
        var id = Guid.NewGuid();

        await _identity.DeleteCharacterAsync(id, UserId);

        await _characterRepository.Received(1).DeleteAsync(id, UserId, Arg.Any<CancellationToken>());
    }
}
