# Skills

Source: free WWN rules, paraphrased.

## Skill list

WWN base skills (21, plus an open `Custom` slot):

`Administer, Connect, Convince, Craft, Exert, Heal, Know, Lead, Magic, Notice, Perform, Pray, Punch, Ride, Sail, Shoot, Sneak, Stab, Survive, Trade, Work`

See `SkillName` enum (`src/WWN.Domain/Enums/SkillName.cs:1`).

> **Enum ordering note**: The original 16 values (Connect through Work, ordinals 0–15) must not be reordered — `FocusEffect.TargetSkill` and `ClassAbilityEffect.TargetSkill` are stored as integers in JSON blobs. The 5 added skills are appended at ordinals 16–20, with `Custom` at 21.

## Ranks

- Range: **−1 (untrained) to 4** — enforced by `SkillRank` value object.
- Untrained = −1 means an untrained character takes a penalty on the check.
- Rank 0 = trained, no bonus. Each rank above 0 is +1 to checks (and to attack rolls when used as a combat skill).

## Skill-check formula

```
check = 2d6 + rank + attribute_mod + focus_skill_bonus + ability_skill_bonus
```

- Attribute is **chosen at the moment of the check** by the player/GM. The same skill can be paired with different attributes (Sneak+DEX vs Sneak+WIS).
- Implementation: `src/WWN.Domain/Rules/SkillCheckCalculator.cs:1` (currently unused — UI does not yet have a check resolver).
- Difficulty bands: roughly 6/8/10/12/14 for easy/routine/challenging/hard/heroic. (See `core-resolution.md`.)

## Skill points / training

- Characters earn **skill points** on level-up to invest in skill ranks. Costs go up with rank (e.g., 1 point for rank 0, 2 for rank 1, 3 for rank 2 — confirm against rulebook for exact costs).
- Expert class grants extra skill points / faster training.
- Skill points may also be spendable to raise an attribute (with constraints). **Not yet modeled** in the codebase.
- Some class features cap skill rank by class or level (e.g., non-Mages cannot raise Magic without specific training).

## App implications

- Setting a rank goes through `Character.SetSkillRank`, which delegates to `CharacterSkill.SetRank` and validates via `SkillRank`'s constructor (-1..4).
- The skill-point economy (available vs. spent) is **not** tracked. Adding it would require new domain state per character (an `int AvailableSkillPoints` plus a transactional API to spend them).
- Custom skills are added with `AddCustomSkill(name, rank)` (each gets a free-text name and `SkillName.Custom`).

## Data model implications

- `CharacterSkill { Name, Rank, CustomName? }` — current shape. Custom skills coexist with named ones in the same list.
- If we add skill-point economy: store `LifetimeSkillPoints` and `SpentSkillPoints`, or a per-rank ledger. Prefer a ledger so we can show an audit trail on the level-up review screen.

## UI implications

- Skill panel groups all 21 base skills + custom skills.
- Surface the attribute-pairing choice when rolling — currently the UI does not roll, but if it ever does, a dropdown of "(skill) + (attribute)" is required.
- For level-up: show "available skill points" with rank-up cost preview.

## Open questions / ambiguities

- Exact skill-point cost table per rank — verify against rulebook before implementing the economy.
- Class-specific skill caps (e.g., Magic for non-magical classes) need a clear policy. Not enforced today.
- Training time / downtime mechanics (skill learning during downtime) are scoped out.
