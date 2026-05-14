using FluentAssertions;
using WWN.Domain.Aggregates;
using WWN.Domain.Enums;
using WWN.Domain.Rules;

namespace WWN.Domain.Tests.Rules;

public class SavingThrowCalculatorTests
{
    // WWN uses 16 - level for all saves (Physical, Evasion, Mental).
    [Theory]
    [InlineData(0, 16)]
    [InlineData(1, 15)]
    [InlineData(2, 14)]
    [InlineData(5, 11)]
    [InlineData(10, 6)]
    public void GetBaseSave_ReturnsExpected(int level, int expected)
    {
        SavingThrowCalculator.GetBaseSave(level).Should().Be(expected);
    }

    [Fact]
    public void Physical_UsesBetterOfStrCon()
    {
        // STR 16 (+1), CON 7 (-1) -> should use +1
        var character = CreateCharacter(str: 16, con: 7);
        SavingThrowCalculator.GetSaveModifier(SaveType.Physical, character).Should().Be(1);
    }

    [Fact]
    public void Evasion_UsesBetterOfDexInt()
    {
        // DEX 10 (0), INT 18 (+2) -> should use +2
        var character = CreateCharacter(dex: 10, intel: 18);
        SavingThrowCalculator.GetSaveModifier(SaveType.Evasion, character).Should().Be(2);
    }

    [Fact]
    public void Mental_UsesBetterOfWisCha()
    {
        // WIS 5 (-1), CHA 5 (-1) -> should use -1
        var character = CreateCharacter(wis: 5, cha: 5);
        SavingThrowCalculator.GetSaveModifier(SaveType.Mental, character).Should().Be(-1);
    }

    [Fact]
    public void GetSaveTarget_CombinesBaseAndModifier()
    {
        // Level 2: base = 16 - 2 = 14, STR 16 (+1) -> target 14 - 1 = 13
        var character = CreateCharacter(str: 16);
        character.SetLevel(2);
        SavingThrowCalculator.GetSaveTarget(SaveType.Physical, character).Should().Be(13);
    }

    [Fact]
    public void Luck_ModifierIsZero()
    {
        // Luck has no attribute modifier regardless of stats
        var character = CreateCharacter(str: 18, dex: 18, con: 18, intel: 18, wis: 18, cha: 18);
        SavingThrowCalculator.GetSaveModifier(SaveType.Luck, character).Should().Be(0);
    }

    [Fact]
    public void Luck_SaveTarget_IsBaseSaveOnly()
    {
        // Level 5: base = 16 - 5 = 11, no modifier -> target 11
        var character = CreateCharacter();
        character.SetLevel(5);
        SavingThrowCalculator.GetSaveTarget(SaveType.Luck, character).Should().Be(11);
    }

    private static Character CreateCharacter(
        int str = 10, int dex = 10, int con = 10,
        int intel = 10, int wis = 10, int cha = 10)
    {
        return Character.Create("Test", CharacterClass.Warrior, new Dictionary<AttributeName, int>
        {
            [AttributeName.Strength] = str,
            [AttributeName.Dexterity] = dex,
            [AttributeName.Constitution] = con,
            [AttributeName.Intelligence] = intel,
            [AttributeName.Wisdom] = wis,
            [AttributeName.Charisma] = cha
        }, "user-1");
    }
}
