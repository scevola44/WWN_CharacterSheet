# App Decisions and Project-Specific Rulings

This file is the **canonical place** for choices the app makes that diverge from, simplify, or interpret the WWN rules. Anything here overrides the generic rules files for the purpose of this codebase.

Format for each entry:

> **Decision** — short title.
> **Source**: `WWN-derived` (paraphrased official rule), `App-specific` (project choice), or `Open` (TODO).
> **Detail** — what the app does. Reference code path when applicable.

---

## Confirmed decisions (encoded in code today)

### Untrained skill rank = −1
**Source**: WWN-derived.
**Detail**: `SkillRank` allows -1..4. Untrained characters take a −1 penalty on rolls. Combat skills follow the same convention (`Stab`, `Shoot`, `Punch` at rank −1 give a −1 to attack rolls).

### Level cap = 10
**Source**: WWN-derived.
**Detail**: Enforced in `Character.SetLevel` and in `SpellSlotCalculator`. Spell-slot table is sized exactly for 1..10.

### Attribute scores 3..18 with table modifier
**Source**: WWN-derived.
**Detail**: `AttributeScore` enforces 3..18; `AttributeModifierTable` returns -2/-1/0/+1/+2 over the bands described in `core-resolution.md`.

### Max System Strain = current CON score
**Source**: WWN-derived.
**Detail**: `Character.SetStrain` validates `0 ≤ current ≤ CON.Score`. **Does not** decrement strain on rest (see Open below).

### AC formula
**Source**: WWN-derived (with project-specific shield interaction).
**Detail**: `10 + DEX_mod + armor_AC + shield(+1 iff body armor equipped AND weapon not two-handed) + focus + ability`. See `CombatCalculator.GetArmorClass`.

### Combat skill defaults
**Source**: App-specific (default mapping; rulebook would call this implicit).
**Detail**: A weapon with `Ranged` tag defaults `CombatSkill = Shoot`; otherwise defaults to `Stab`. `Punch` is selected only when explicitly configured (unarmed, monk-style builds).

### Effort pool formula
**Source**: WWN-derived.
**Detail**: `max(1, 1 + INT_mod + max(0, Magic_rank))`, capped below at 1. Implemented in `EffortPoolCalculator`. Untrained Magic (rank −1) is treated as 0 here.

### Rest semantics
**Source**: App-specific (subset of full WWN rest rules).
**Detail**: `Character.RestForDay()` clears scene + day Effort and restores all spell slots. `EndScene()` clears scene Effort only. **Does not** heal HP or reduce strain (see Open below).

### Adventurer requires two partial classes
**Source**: WWN-derived.
**Detail**: `Character.Create` rejects Adventurer without both partials, and rejects non-Adventurer with any partials.

### Magic eligibility = Mage OR Adventurer with Partial Mage
**Source**: WWN-derived (simplified).
**Detail**: Both High Magic (spells/slots) and Low Magic (Arts/Effort) gate on the same predicate (`EffortPoolCalculator.HasArts`). The rulebook's distinct Mage traditions are flattened into a single "magical" capability for now.

### INT bonus to slots: Mage only
**Source**: App-specific.
**Detail**: `SpellSlotCalculator.CalculateSlots` adds positive INT mod to the 1st-level slot count for Mages, **not** Partial Mages. **Verify against rulebook.**

### Adventurer-with-Partial-Warrior BAB = level/2 + 1
**Source**: App-specific (questionable).
**Detail**: `CombatCalculator.GetBaseAttackBonus` gives this case a small edge over the default `level/2`. **Verify against rulebook.** WWN's official partial-warrior BAB rule should be checked before relying on this.

### Spell-slot usage clones the array
**Source**: App-specific (EF Core constraint).
**Detail**: `Character.UseSpellSlot` allocates a new array because EF Core's change tracker uses reference equality. Preserve this pattern when extending.

---

## Open / undecided (TODOs)

### Specialist save assignment per class
**Source**: Open.
**Detail**: `SavingThrowCalculator.GetBaseSave` accepts `isSpecialist` as a parameter, but no class→save mapping is encoded. Decide which class is "specialist" in which save (Warrior=Physical? Expert=Evasion? Mage=Mental? Adventurer=?), then either persist it on the character or derive from class + partials.

### HP recovery on rest
**Source**: Open.
**Detail**: `RestForDay` does not heal HP. Pick a rule: full heal? `level × 1` HP? Fraction of max? Then implement and document.

### System Strain decay on rest
**Source**: App-specific.
**Detail**: Strain does **not** auto-decrement on rest. It is decremented manually via the `+1 / −1` controls in `StrainTracker`. `RestForDay()` does not touch `CurrentStrain`. The UI warns at max strain and labels it "Decremented manually".

### Encumbrance totals and capacity
**Source**: Open.
**Detail**: README lists "Total Encumbrance Display" as missing. Suggested: `MaxReadied = STR/2` (round down), `MaxStowed = STR`. Aggregate `Item.Encumbrance` per slot type. Decide whether equipped armor/weapon counts toward readied or is exempt.

### Initiative
**Source**: Open.
**Detail**: Not implemented. Default to `1d8 + DEX_mod` (group-initiative variant common in WWN) and let foci/abilities apply via the existing `FocusEffectType.Initiative`.

### Skill-point economy
**Source**: Open.
**Detail**: No `AvailableSkillPoints` on `Character`. Decide cost-per-rank table, level-up grants per class, Expert bonus, and whether attribute increases from skill points are supported.

### Distinct partial classes
**Source**: Open.
**Detail**: Domain does not enforce `PartialClassA != PartialClassB`. Decide whether duplicates are allowed; if not, add validation in `Character.Create`.

### Heavy armor / spell failure
**Source**: Open.
**Detail**: WWN penalizes mages casting in heavy armor. Not modeled. Would require an `ArmorWeightClass` enum.

### Death and dying
**Source**: Open.
**Detail**: Currently HP clamps at 0 with no further state. No mortal-wound timer, no stabilization mechanic.

### Wealth / treasure
**Source**: Open.
**Detail**: No coin or banking model. Tracked in README as low priority.

### Status conditions (poisoned, prone, frightened, etc.)
**Source**: Open.
**Detail**: Not modeled. Would belong as a list of typed `StatusCondition` records on `Character`.

---

## Style rule

When adding a new entry: keep it short. Title + one of {WWN-derived, App-specific, Open} + 1-3 sentence detail + a code reference where applicable. If it grows beyond that, promote it to its own rule file in `.claude/rules/` and link.
