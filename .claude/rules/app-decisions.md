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

### Adventurer's two partial classes must be distinct
**Source**: WWN-derived.
**Detail**: `Character.Create` rejects `(PartialMage, PartialMage)` and any other duplicate pair. See `src/WWN.Domain/Aggregates/Character.cs:105-106`.

### Magic eligibility = Mage OR Adventurer with Partial Mage
**Source**: WWN-derived (simplified).
**Detail**: Both High Magic (spells/slots) and Low Magic (Arts/Effort) gate on the same predicate (`EffortPoolCalculator.HasArts`). The rulebook's distinct Mage traditions are flattened into a single "magical" capability for now.

### INT bonus to slots: Mage only
**Source**: App-specific.
**Detail**: `SpellSlotCalculator.CalculateSlots` adds positive INT mod to the 1st-level slot count for Mages, **not** Partial Mages. **Verify against rulebook.**

### All saves use 16 − level − attribute_mod
**Source**: WWN-derived.
**Detail**: Physical = 16−level−max(STR,CON), Evasion = 16−level−max(DEX,INT), Mental = 16−level−max(WIS,CHA). There is no "generalist" formula. The `isSpecialist` parameter has been removed from `SavingThrowCalculator`. A Luck save (16−level, no attribute mod) exists in the rulebook but is not yet modeled.

### Adventurer-with-Partial-Warrior BAB = level/2 + 1
**Source**: App-specific (confirmed project choice).
**Detail**: `CombatCalculator.GetBaseAttackBonus` gives this case a slight edge over the baseline half-progression (`level/2`). The exact rulebook BAB table for Partial Warrior should be verified before building further level-up logic on top of this formula.

### Spell-slot usage clones the array
**Source**: App-specific (EF Core constraint).
**Detail**: `Character.UseSpellSlot` allocates a new array because EF Core's change tracker uses reference equality. Preserve this pattern when extending.

### Multi-tenancy unit = Campaign
**Source**: App-specific.
**Detail**: A single concept, `Campaign`, owns membership, content scope, and character grouping. No workspace/org layer above it. See `multi-tenancy.md`.

### GameMaster is a per-campaign role, not a global Identity role
**Source**: App-specific.
**Detail**: `CampaignMembership.Role ∈ {GameMaster, Player}`. The only global Identity role remains `Admin` (system-content authoring, operational tooling). See `multi-tenancy.md`.

### A character belongs to at most one campaign at a time
**Source**: App-specific.
**Detail**: `Character.CampaignId Guid?`; `null` = personal. Move semantics preserve `CharacterId` (same row, just update `CampaignId`).

### Personal characters are always PlayerCharacter
**Source**: App-specific.
**Detail**: `Character.CharacterRole` is non-nullable, defaults to `PlayerCharacter`. `GmNpc` requires `CampaignId IS NOT NULL` as a domain invariant. See `multi-tenancy.md`.

### Player leaving a campaign orphans their characters back to personal
**Source**: App-specific.
**Detail**: On `RemoveMember`, every character of that user with the matching `CampaignId` has `CampaignId` set to `null` and role normalized to `PlayerCharacter`. GM stops seeing them from the next request.

### Campaign deletion orphans GM NPCs to the former GM as personal PCs
**Source**: App-specific.
**Detail**: On campaign delete, `GmNpc` rows have `CampaignId = null` and role coerced to `PlayerCharacter`; `OwnerUserId` is unchanged. Symmetric with the player-leaves rule.

### GM transfer leaves NPC ownership unchanged
**Source**: App-specific.
**Detail**: `Campaign.TransferGameMaster` only swaps roles; `Character.OwnerUserId` is not modified. New GM gets write access via the GameMaster policy. If the original GM later leaves, their NPCs orphan back to them as personal PCs.

### Content gains nullable CampaignId; null = system / SRD
**Source**: App-specific.
**Detail**: `FocusDefinition`, `Spell`, `Art`, **and `ArtSource`** all add a nullable `CampaignId`. Existing rows pre-migration stay `NULL` and remain visible to everyone. `ArtSource` keeps its `int` Id despite the asymmetry with the others.

### System-content writes are Admin-only; campaign-content writes are campaign-GM-only
**Source**: App-specific.
**Detail**: Phase 3 closes today's no-auth gap on `FocusDefinitionEndpoints` / `SpellEndpoints` / `ArtEndpoints` content CRUD by requiring authorization, with policy chosen by the row's `CampaignId` (`null` ⇒ Admin; set ⇒ GameMaster of that campaign).

### Active campaign is a query parameter, not server state
**Source**: App-specific.
**Detail**: Content-browse endpoints accept `?campaignId=` (validated against the requester's memberships). Frontend tracks the active campaign in React context, persisted to `localStorage`. No cookie, no DB column.

### Authorization is centralized in access policies
**Source**: App-specific.
**Detail**: `ICharacterAccessPolicy` / `ICampaignAccessPolicy` / `IContentAccessPolicy` (pure functions of user, target, memberships). Endpoints and services consume them; rules are not duplicated per endpoint.

### Invite codes: 12-char URL-safe, 7-day default, unlimited uses, soft-revocable
**Source**: App-specific.
**Detail**: Alphabet `a-z2-9` (no `0/1/o/l/i`). Default `ExpiresAt = now + 7d`, default `MaxUses = null` (unlimited), revocation sets `RevokedAt` (soft). Redeeming as an existing member is an idempotent success. See `multi-tenancy.md`.

### Forbidden vs Not Found on hidden characters
**Source**: App-specific.
**Detail**: Return `403 Forbidden` when the requester could plausibly have learned the character ID (e.g., a former campaign-mate); return `404` when the ID is structurally unknowable to them. Avoids leaking existence in the common case.

---

## Open / undecided (TODOs)

### HP recovery on rest
**Source**: Open.
**Detail**: `RestForDay` does not heal HP. Pick a rule: full heal? `level × 1` HP? Fraction of max? Then implement and document.

### System Strain decay on rest
**Source**: App-specific.
**Detail**: Strain does **not** auto-decrement on rest. It is decremented manually via the `+1 / −1` controls in `StrainTracker`. `RestForDay()` does not touch `CurrentStrain`. The UI warns at max strain and labels it "Decremented manually".

### Encumbrance totals and capacity
**Source**: App-specific (WWN-derived formulas, project-specific decisions below).
**Detail**: `MaxReadied = STR_score / 2` (integer division, round down); `MaxStowed = STR_score` (separate pool). Load = `Σ(Item.Encumbrance × Item.Quantity)` per slot group. **Equipped items count toward readied load** (armor on body and weapon in hand are part of your readied carry). Over-cap is a **warning only** — no API enforcement, no hard block. Implemented in `src/WWN.Domain/Rules/EncumbranceCalculator.cs`; surfaced via `EncumbranceSummaryDto` on `CharacterDetailDto`; displayed in `InventoryPanel.tsx` with red text when over cap.

### Initiative
**Source**: Open.
**Detail**: Not implemented. Default to `1d8 + DEX_mod` (group-initiative variant common in WWN) and let foci/abilities apply via the existing `FocusEffectType.Initiative`.

### Skill-point economy
**Source**: Open.
**Detail**: No `AvailableSkillPoints` on `Character`. Decide cost-per-rank table, level-up grants per class, Expert bonus, and whether attribute increases from skill points are supported.

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
