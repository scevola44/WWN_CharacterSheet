using FluentAssertions;
using WWN.Domain.Aggregates;
using WWN.Domain.Enums;
using WWN.Domain.Rules;

namespace WWN.Domain.Tests.Rules;

public class SavingThrowCalculatorTests
{
    [Theory]
    [InlineData(0, false, 15)]
    [InlineData(1, false, 15)]
    [InlineData(2, false, 14)]
    [InlineData(3, false, 14)]
    [InlineData(4, false, 13)]
    [InlineData(10, false, 10)]
    public void GetBaseSave_NonSpecialist_ReturnsExpected(int level, bool specialist, int expected)
    {
        SavingThrowCalculator.GetBaseSave(level, specialist).Should().Be(expected);
    }

    [Theory]
    [InlineData(0, true, 16)]
    [InlineData(1, true, 15)]
    [InlineData(5, true, 11)]
    [InlineData(10, true, 6)]
    public void GetBaseSave_Specialist_ReturnsExpected(int level, bool specialist, int expected)
    {
        SavingThrowCalculator.GetBaseSave(level, specialist).Should().Be(expected);
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
        // Level 2 non-specialist: base 14, STR 16 (+1) -> target 13
        var character = CreateCharacter(str: 16);
        character.SetLevel(2);
        SavingThrowCalculator.GetSaveTarget(SaveType.Physical, character, false).Should().Be(13);
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
        });
    }
}
