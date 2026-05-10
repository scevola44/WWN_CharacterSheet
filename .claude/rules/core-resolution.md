# Core Resolution

Source: free WWN rules, paraphrased.

## Roll types (raw rule summary)

| Roll | Dice | Goal | Modifier sources |
|------|------|------|------------------|
| Attack roll | `1d20 + attack bonus` | meet/exceed target AC | BAB + combat-skill rank + relevant attribute mod + situational |
| Saving throw | `1d20` | meet/exceed save target | save target = 16 − level − best of two attribute mods |
| Skill check | `2d6 + skill rank + attribute mod` | meet difficulty | rank −1..4, attribute mod, focus/ability bonuses |
| Damage | weapon die + attribute mod + bonuses | — | Stab/Shoot/Punch usually use STR or DEX |

Skill checks are **2d6** in WWN, not d20. Difficulties cluster around 6 (easy), 8 (routine), 10 (challenging), 12 (hard), 14 (very hard).

## Save targets (summary)

- Three saves: **Physical**, **Evasion**, **Mental** (see `SaveType` enum).
- All saves use `16 − level − attribute_mod` (confirmed per rulebook; see `app-decisions.md`).
- Modifier per save uses the **better of two attribute mods**:
  - Physical: max(STR mod, CON mod)
  - Evasion: max(DEX mod, INT mod)
  - Mental: max(WIS mod, CHA mod)
- Final target = `16 − level − modifier`. Roll `≥` target succeeds.
- A **Luck save** (16 − level, no attribute mod) exists in the rulebook but is not yet modeled.
- Implementation: `src/WWN.Domain/Rules/SavingThrowCalculator.cs:1`.

## Attribute modifier table

| Score | Modifier |
|-------|----------|
| 3 | −2 |
| 4–7 | −1 |
| 8–13 | 0 |
| 14–17 | +1 |
| 18 | +2 |

Implementation: `src/WWN.Domain/Rules/AttributeModifierTable.cs:1`. Scores are constrained to 3–18 by `AttributeScore`.

## App implications

- Server holds **scores and ranks**, not roll outcomes. Dice rolling is currently UI-side or absent.
- Save target is a function of (level, attributes). Cache or recompute on character change; cheap.
- For attack rolls the attacker chooses the combat skill / attribute on the weapon (see `combat.md`).

## Data model implications

- Skill checks need an explicit **(skill, attribute)** pairing because WWN allows the GM to pick the relevant attribute.

## UI implications

- Show base save, attribute mod, and final target separately so players can sanity-check.
- For skill checks, surface the "rolling 2d6 + X" formula explicitly; don't reuse d20 wording.

## Open questions / ambiguities

- The codebase currently does not own a generic dice-roller. If we add one, we need to decide whether to keep rolls server- or client-side (auditability vs. latency).
- A **Luck save** (16 − level, no attribute mod) exists in the rulebook but is not yet modeled.
