# App Decisions

Design questions resolved by cross-checking the Worlds Without Number free PDF.

---

## Base Attack Bonus (BAB)

**Question:** Which BAB formula should each class use?

**Decision:** Three distinct progressions exist in the rules (free PDF class tables, pp. 26–35).

| Level | Warrior | Expert | Mage |
|-------|---------|--------|------|
| 1     | +1      | +0     | +0   |
| 2     | +2      | +1     | +0   |
| 3     | +3      | +1     | +0   |
| 4     | +4      | +2     | +0   |
| 5     | +5      | +2     | +1   |
| 6     | +6      | +3     | +1   |
| 7     | +7      | +3     | +1   |
| 8     | +8      | +4     | +1   |
| 9     | +9      | +4     | +1   |
| 10    | +10     | +5     | +2   |

- **Warrior**: `level` (integer)
- **Expert**: `floor(level / 2)` — C# integer division `level / 2`
- **Mage**: `floor(level / 5)` — C# integer division `level / 5`

**Adventurer (multi-class):**

The base Adventurer BAB uses the Expert progression (`floor(level / 2)`), regardless of
which non-Warrior partial classes are chosen. Adding **Partial Warrior** grants a permanent
+1 on top of that:

- Adventurer (no Partial Warrior): `floor(level / 2)`
- Adventurer (Partial Warrior): `floor(level / 2) + 1`

**Implementation:** `CombatCalculator.GetBaseAttackBonus` in
`src/WWN.Domain/Rules/CombatCalculator.cs`.

---

## Mage INT Modifier and Spell Slots

**Question:** Does the Intelligence modifier add bonus 1st-level spell slots for Partial
Mage characters (Adventurers with PartialMage), or only for full Mages?

**Decision:** The bonus applies to **full Mages only**. The WWN free rules grant full Mages
a number of bonus 1st-level slots equal to their INT modifier (when positive). Partial Mage
(Adventurer with PartialMage) does not receive this bonus.

**Implementation:** `SpellSlotCalculator.CalculateSlots` in
`src/WWN.Domain/Rules/SpellSlotCalculator.cs` already implements this correctly — the
`intModifier` is applied only when `charClass == CharacterClass.Mage`.
