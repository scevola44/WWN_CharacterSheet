# Classes and Leveling

Source: free WWN rules, paraphrased.

## Classes (raw rule summary)

| Class | Hit-die mod | BAB progression | Magic | Notes |
|-------|-------------|-----------------|-------|-------|
| Warrior | +2 | full (=level) | none | Best fighter; full BAB; bonus combat foci |
| Expert | 0 | half (level/2) | none | Skill specialist; extra free skill picks; bonus non-combat foci |
| Mage | −1 | half (level/2) | High Magic (spells & slots) | Wizards or other Mage traditions |
| Adventurer | depends on partials | depends on partials | depends on partials | Picks **two** distinct partial classes |

Partial classes: **Partial Warrior**, **Partial Expert**, **Partial Mage**. An Adventurer gets two and combines their effects (limited versions of the parent-class abilities).

### Hit-die modifier (implementation)

`src/WWN.Domain/Rules/HitPointCalculator.cs:1` — Warrior +2, Mage −1, Expert 0, Adventurer takes +2 if partial Warrior, −1 if partial Mage, else 0. The base hit die is `1d6`.

### Base Attack Bonus

`src/WWN.Domain/Rules/CombatCalculator.cs:1`:

- Warrior: `BAB = level`.
- Adventurer **with** Partial Warrior: `BAB = level/2 + 1` (project-specific +1 edge).
- Everyone else: `BAB = level/2`.

> **App-specific note**: the `+1` for Partial-Warrior Adventurers is a code choice. WWN partial classes typically grant Partial Warrior the full BAB progression. **Verify against the rulebook** before relying on this — see `app-decisions.md`.

## Saves: specialist vs generalist

- Generalist save target = `15 − floor(level/2)`.
- Specialist save target = `16 − level` (improves faster with level).
- Which save each class is "specialist" in is a per-class design decision; the codebase passes `isSpecialist` as a parameter and does not currently bind specialist saves to specific classes. **Tracked as TODO** in `app-decisions.md`.

## XP and level cap

- Level cap: **10** (enforced in `Character.SetLevel` and `SpellSlotCalculator`).
- WWN uses an XP-per-level table; the codebase stores raw `ExperiencePoints` but does **not** auto-level the character. Leveling is a manual operation in the UI.

## Class abilities and foci on level-up

- Class abilities live as `ClassAbilityDefinition` entities; characters load applicable definitions via `LoadClassAbilityDefinitions(...)` on the aggregate. Effects feed into combat calcs through `ClassAbilityEffectAggregator`.
- Characters gain new foci at certain levels (typically every odd level for full classes, slower for Adventurers). The exact cadence is **not yet enforced** in code — focus assignment is manual.

## App implications

- Level-up flow should:
  1. Increase level (1..10, capped).
  2. Recompute HP (manual or assisted roll; min 1 per level recommended).
  3. Recompute BAB, save targets, spell slots, Effort pool — all already handled by calculators (called on demand, not stored).
  4. Apply newly available class abilities (filter `ClassAbilityDefinition` by required level).
  5. Optionally prompt for new skill picks / foci according to class progression (TODO).
- BAB, AC, attack bonus, save targets, spell slots, Effort pool are **derived** every time. Don't persist them.

## Data model implications

- `Character.Class` plus optional `PartialClassA`/`PartialClassB` is the canonical class state.
- No `IsSpecialistSave` field exists; if needed, add `Dictionary<SaveType, bool>` or derive from class.
- `ClassAbilityDefinition` is a content table; class-ability effects flow via `ClassAbilityEffect` value objects.

## UI implications

- Display BAB, save targets, HP die roll, and (for casters) slot table on the sheet.
- Level-up wizard: new level → HP gain → ability/focus picks → confirm.
- For Adventurers, render both partial-class names and inherited features clearly.

## Open questions / ambiguities

- Specialist-save mapping per class is unimplemented (see `app-decisions.md`).
- Partial-Warrior BAB: confirm `+1` choice against rulebook (could be a regression of the generalist `level/2` formula).
- Level-up cadence for foci/skills is not encoded — currently free-form.
- Multiclassing beyond Adventurer is not supported by the rules and is intentionally absent from the model.
