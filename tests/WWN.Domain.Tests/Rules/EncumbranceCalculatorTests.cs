using FluentAssertions;
using WWN.Domain.Entities;
using WWN.Domain.Enums;
using WWN.Domain.Rules;

namespace WWN.Domain.Tests.Rules;

public class EncumbranceCalculatorTests
{
    // --- MaxReadied ---

    [Theory]
    [InlineData(10, 5)]
    [InlineData(3,  1)]   // STR floor; 3/2 = 1
    [InlineData(18, 9)]
    [InlineData(7,  3)]   // odd score rounds down
    public void MaxReadied_IsHalfStrRoundedDown(int str, int expected)
    {
        EncumbranceCalculator.GetMaxReadied(str).Should().Be(expected);
    }

    // --- MaxStowed ---

    [Theory]
    [InlineData(10, 10)]
    [InlineData(3,  3)]
    [InlineData(18, 18)]
    public void MaxStowed_IsStrScore(int str, int expected)
    {
        EncumbranceCalculator.GetMaxStowed(str).Should().Be(expected);
    }

    // --- GetReadiedLoad ---

    [Fact]
    public void ReadiedLoad_SumsReadiedAndEquippedItems()
    {
        var items = new List<Item>
        {
            MakeItem(encumbrance: 2, qty: 1, slot: ItemSlotType.Readied),
            MakeItem(encumbrance: 1, qty: 1, slot: ItemSlotType.Equipped),
            MakeItem(encumbrance: 5, qty: 1, slot: ItemSlotType.Stowed),   // ignored
        };

        EncumbranceCalculator.GetReadiedLoad(items).Should().Be(3);
    }

    [Fact]
    public void ReadiedLoad_MultipliesEncumbranceByQuantity()
    {
        var items = new List<Item>
        {
            MakeItem(encumbrance: 1, qty: 3, slot: ItemSlotType.Readied),  // 3 slots
        };

        EncumbranceCalculator.GetReadiedLoad(items).Should().Be(3);
    }

    [Fact]
    public void ReadiedLoad_EmptyInventory_ReturnsZero()
    {
        EncumbranceCalculator.GetReadiedLoad([]).Should().Be(0);
    }

    // --- GetStowedLoad ---

    [Fact]
    public void StowedLoad_SumsOnlyStowedItems()
    {
        var items = new List<Item>
        {
            MakeItem(encumbrance: 3, qty: 2, slot: ItemSlotType.Stowed),   // 6
            MakeItem(encumbrance: 2, qty: 1, slot: ItemSlotType.Readied),  // ignored
            MakeItem(encumbrance: 1, qty: 1, slot: ItemSlotType.Equipped), // ignored
        };

        EncumbranceCalculator.GetStowedLoad(items).Should().Be(6);
    }

    [Fact]
    public void StowedLoad_MultipliesEncumbranceByQuantity()
    {
        var items = new List<Item>
        {
            MakeItem(encumbrance: 2, qty: 4, slot: ItemSlotType.Stowed),  // 8 slots
        };

        EncumbranceCalculator.GetStowedLoad(items).Should().Be(8);
    }

    // --- Over-cap detection (load > max) ---

    [Fact]
    public void OverReadied_WhenLoadExceedsMax()
    {
        var items = new List<Item>
        {
            MakeItem(encumbrance: 1, qty: 6, slot: ItemSlotType.Readied),
        };
        var maxReadied = EncumbranceCalculator.GetMaxReadied(strScore: 10); // 5

        var overCap = EncumbranceCalculator.GetReadiedLoad(items) > maxReadied;
        overCap.Should().BeTrue();
    }

    [Fact]
    public void NotOverReadied_WhenLoadEqualsMax()
    {
        var items = new List<Item>
        {
            MakeItem(encumbrance: 1, qty: 5, slot: ItemSlotType.Readied),
        };
        var maxReadied = EncumbranceCalculator.GetMaxReadied(strScore: 10); // 5

        var overCap = EncumbranceCalculator.GetReadiedLoad(items) > maxReadied;
        overCap.Should().BeFalse();
    }

    // --- Helpers ---

    private static Item MakeItem(int encumbrance, int qty, ItemSlotType slot)
    {
        var item = new Item("Test", encumbrance, slot, qty);
        return item;
    }
}
