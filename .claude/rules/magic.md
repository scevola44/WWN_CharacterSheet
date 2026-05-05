# Magic

Source: free WWN rules, paraphrased.

WWN distinguishes **High Magic** (Mage spells, Vancian-style slots) from **Low Magic / Arts** (other traditions using **Effort** as a resource). The codebase models both.

## High Magic — spells & slots

### Who has it

- **Mage** class: full spell progression.
- **Adventurer** with **Partial Mage**: reduced spell progression.
- All other classes: none.

`EffortPoolCalculator.HasArts()` and the existence of `KnownArts`/`KnownSpell` track these capabilities.

### Spell levels

Spells exist at levels **1 through 6**. Slots are per spell level.

### Slot table

Encoded literally in `src/WWN.Domain/Rules/SpellSlotCalculator.cs:1`. Summary:

- **Mage** at level 1: `[1,0,0,0,0,0]` → grows to level 10: `[2,2,2,2,2,1]`.
- **Partial Mage** (Adventurer): starts level 1 with **no** slots; grows to level 10: `[1,1,1,1,1,0]`.
- **INT bonus**: a positive INT modifier adds extra **1st-level** slots — **Mage only** in this codebase. Partial Mages do not get the INT bonus per current implementation. (Verify against rulebook; tracked in `app-decisions.md`.)

### Slot usage / rest

- `Character.UseSpellSlot(level)` increments `SpellSlotsUsed[level-1]` (1..6 input, 0..5 array index). It clones the array because EF Core change-tracking compares array references.
- `RestoreAllSpellSlots()` zeroes the array. Called by `RestForDay()`.
- "Available" slots = `Calculate(...)[i] − SpellSlotsUsed[i]`. The aggregate exposes used; available is computed UI-side.

### Spellbook

- A character has a list of `KnownSpell` (linked to `Spell` definitions). `LearnSpell` rejects duplicates by `SpellId`. `ForgetSpell` removes one.
- Casting a known spell consumes a slot of **at least** the spell's level (upcasting via higher slots is a typical WWN allowance — confirm before implementing).

## Low Magic — Arts and Effort

### Who has it

Same eligibility as High Magic in this codebase: Mage or Adventurer with Partial Mage. WWN's actual rules tie Arts to specific traditions/classes; **the project simplifies "magical" classes into one Effort pool**. See `app-decisions.md`.

### Effort pool

`src/WWN.Domain/Rules/EffortPoolCalculator.cs:1`:

```
max_effort = max(1, 1 + INT_mod + max(0, Magic_skill_rank))
```

- `Magic_skill_rank` is the character's rank in the `Magic` skill, floored to 0 (untrained = −1 is treated as 0 for this formula).
- Floored at 1 — even an untrained mage has at least 1 Effort.

### Effort commitment

`EffortCommitment` enum: `Scene`, `Day`, `Sustained`.

| Commitment | Released by |
|------------|-------------|
| Scene | `EndScene()` (clears scene effort) |
| Day | `RestForDay()` (clears day + scene + restores spell slots) |
| Sustained | `ReleaseSustainedEffort(amount)` — explicitly released by player |

`CommitEffort(commitment, maxEffort, amount=1)` validates that `scene+day+sustained+amount ≤ maxEffort`. Free Effort = `max − total_committed`.

### Arts

- `KnownArt` is the per-character record (linked to `Art` content). Arts are gained at level-up; specific Arts have their own activation costs (commit Effort for scene/day/etc., or as one-shot scene effects).

## App implications

- Slot count and Effort pool are derived from class/level/attribute/skill on demand; never persist.
- Rest semantics: `RestForDay` covers "sleep / overnight rest" — clears scene+day Effort and restores all spell slots. Sustained Effort is **not** released by sleeping.

## Data model implications

- Spell-slot **usage** (`int[6]`) is persisted; max is derived.
- Effort commitments are three integers on `Character` (scene/day/sustained). Total committed = sum.
- Adding "spell preparation" (if WWN supports it) would mean tagging which spells in the spellbook are prepared today — currently not modeled; `KnownSpell` is enough.

## UI implications

- Spell-slot tracker: per spell level, show `[used / max]` and a row of pip toggles.
- Effort tracker: max, free, scene, day, sustained — with "End scene", "Rest for day", "Release sustained" controls (these already exist as components: `EffortTracker.tsx`).
- Hide spell/Effort UI entirely for non-casters.

## Open questions / ambiguities

- INT bonus to slots applies to Mage only in code; rulebook may extend this to Partial Mages — verify.
- Upcasting (using a higher-level slot to cast a lower-level spell) is not modeled.
- Heavy-armor casting penalty for High Magic is not enforced.
- Some Arts have complex resource costs (committing Effort and a separate Strain hit, or Effort scaling with target HD); only the slot/commitment shape is generic.
- The "all magical classes share one Effort pool" simplification may not match every WWN tradition (some have separate pools per tradition).
