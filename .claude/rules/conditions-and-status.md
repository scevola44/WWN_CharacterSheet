# Conditions and Status

Source: free WWN rules, paraphrased.

Covers: Hit Points, dying/dead state, **System Strain**, healing, rest cycles.

## Hit Points

- `Character.MaxHitPoints` and `CurrentHitPoints` are persisted ints. `SetHitPoints`, `TakeDamage`, `Heal` are the mutators.
- HP gain per level is `1d6 + CON_mod + class_hit_die_mod`, with a typical "minimum 1 per level" rule. Class hit-die mod table is in `classes-and-leveling.md`.
- Reaching 0 HP knocks a character out. WWN has rules around mortal wounds and stabilization (death-and-dying timer; medic checks). **Not modeled in code yet** — we just clamp to 0.

## System Strain

A core WWN mechanic: characters accumulate Strain when they receive non-natural healing, use certain abilities, or push themselves. Strain is not damage; it caps how much "magical/medical patching" they can absorb before needing real rest.

- **Max strain = current CON score** (not modifier). Enforced by `Character.SetStrain` (`src/WWN.Domain/Aggregates/Character.cs:33`).
- Stored as `CurrentStrain` (default 0).
- A character at max strain cannot receive further strain-causing effects (e.g. healing magic) until rest reduces strain.
- Strain typically decreases by **1 per day of rest** in safe conditions (rulebook specifics may vary).

> The codebase enforces the cap and floor (0 ≤ current ≤ CON), but **does not auto-decrement on rest**. `RestForDay()` does not currently reduce strain. See `app-decisions.md`.

## Healing

WWN healing categories:

| Category | Restores HP? | Causes strain? |
|----------|--------------|----------------|
| Natural rest (full night) | partial / full per rulebook | no |
| Bandages / first aid (in-combat) | small amount | no |
| Magical healing (spells, Arts) | yes | yes (per use) |
| Stims / drugs | yes | yes |

The **rate of HP recovery from rest** in WWN is roughly: full HP restored after a full night of safe sleep (subject to GM ruling); shorter rests give partial recovery. Confirm before implementing.

## Rest cycles (codebase)

`Character.RestForDay()` currently:

- Clears scene Effort.
- Clears day Effort.
- Restores all spell slots to full.

It does **not** currently:

- Heal HP.
- Reduce System Strain.
- Release sustained Effort (intentional — sustained Effort persists across rests).

`Character.EndScene()` clears scene Effort only (used after a combat encounter).

## App implications

- HP tracker exists (`HitPointTracker.tsx`); strain tracker component exists (`StrainTracker.tsx`) — wire it to `CurrentStrain` and `CON_score`.
- Rest UI should expose at least two buttons: "End scene" and "Rest for day" (which may need to also reduce strain and heal HP — pending product decision).

## Data model implications

- `CurrentStrain` is on the aggregate; max is derived.
- If we model "permanent strain" (some WWN abilities cause permanent strain that lowers max), we'd need a `PermanentStrain` int and adjust max accordingly: `effective_max = CON − PermanentStrain`.

## UI implications

- Strain bar with `[current / CON]` and a +/− control.
- Warning state at strain ≥ CON.
- Rest button(s) should clearly summarize what they do (slot restore? HP heal? strain decrement?).

## Open questions / ambiguities

- HP recovery rate per rest type — pick a default and document in `app-decisions.md`.
- Strain decrement on rest — default policy not chosen.
- Death and dying / mortal wounds — not modeled. Consider a `DeathState` enum (`Healthy | Unconscious | MortallyWounded | Dead`).
- Permanent strain (from certain Arts) — not modeled.
- Per-condition "tags" (poisoned, frightened, encumbered, prone) are not modeled at all yet; if added they should live as a `List<StatusCondition>` with definitions in a content table.
