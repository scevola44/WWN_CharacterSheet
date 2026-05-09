# WWN Rules — Index

Modular, retrieval-oriented summaries of Worlds Without Number rules, written for an AI coding agent.

## Source & legality

Notes are paraphrased from the **free** rules edition of *Worlds Without Number* (Sine Nomine Publishing) and the publicly available SRD-equivalent material. **No long passages are copied verbatim.** Mechanics are restated in original wording and normalized for implementation. When a rule is unclear or only available in paid material, the file says so under "Open questions".

## How to use these files

1. Read the **table below** and pick only the files you need for the task.
2. Read the file. Treat the "App implications" / "Data model" / "UI" sections as the source of truth for how this codebase chose to interpret the rule.
3. Check `app-decisions.md` for project-specific rulings.
4. Cross-link to existing C# in `src/WWN.Domain/Rules/` — most calculators already exist.

## Files

| File | Covers | Read when... |
|------|--------|--------------|
| `core-resolution.md` | d20 attack/save mechanics, skill checks (2d6+mod), advantage-style rolls | Anything that resolves a roll |
| `character-creation.md` | Attributes, backgrounds, foci at chargen, classes & partial classes, starting HP | Character creation, randomizers, validation |
| `classes-and-leveling.md` | Class features, partial-class combos, BAB, HP per level, XP table, level-up gating | Level-up flow, class-feature display, HP recompute |
| `combat.md` | AC, attack rolls, damage, **shock**, weapon tags, initiative | Attack pipeline, weapon UI, AC calc |
| `skills.md` | Skill list, ranks (-1..4), skill points / training, skill-check formula | Skill panel, level-up skill picks, custom skills |
| `equipment-and-encumbrance.md` | Item slots (Stowed/Readied/Equipped), encumbrance limits, armor, shields | Inventory UI, encumbrance bar, equip rules |
| `magic.md` | Mage spells & slots, Adventurer partial Mage, **Effort** for Arts, High/Low Magic distinction | Spell UI, slot tracker, Effort tracker, Arts |
| `conditions-and-status.md` | HP / dying / death, **System Strain**, healing, rest cycles | HP tracker, Strain tracker, rest buttons |
| `multi-tenancy.md` | **Not WWN.** Campaigns, memberships, invites, character/content visibility, GM vs Player vs Admin | Anything that touches campaigns, sharing, authorization, or content scoping |
| `app-decisions.md` | Project-specific rulings, simplifications, TODOs | Always, briefly, before coding rule-heavy features |

## Common cross-links

- **Combat ↔ Skills**: weapon attack rolls use a combat skill (Stab/Shoot/Punch).
- **Combat ↔ Equipment**: AC depends on worn armor + shield; two-handed weapons block shield bonus.
- **Magic ↔ Classes**: which spell list / Effort pool a character has depends on Mage / Partial Mage status.
- **Magic ↔ Conditions**: spell slots and Effort scene/day commitments reset on rest (see `RestForDay()` in `Character.cs`).
- **Skills ↔ Classes**: Expert gets extra skill picks; specific class features reference skill ranks.
- **Conditions ↔ Equipment**: System Strain caps gear like stims/healing; encumbrance affects movement penalties.
- **Multi-tenancy ↔ everything**: every character has a (nullable) `CampaignId`; every content row (Foci/Spells/Arts/ArtSources) has a (nullable) `CampaignId`; visibility is computed from the viewer's campaign memberships.

## Minimum files to read for task X

| Task | Files to read |
|------|---------------|
| Render character sheet (read-only) | `character-creation.md`, `classes-and-leveling.md`, `combat.md`, `skills.md`, `equipment-and-encumbrance.md`, `app-decisions.md` |
| Character creation form | `character-creation.md`, `classes-and-leveling.md`, `skills.md`, `app-decisions.md` |
| Level-up flow | `classes-and-leveling.md`, `skills.md`, `magic.md` (if caster), `conditions-and-status.md` (HP), `app-decisions.md` |
| Attack resolution / weapon UI | `combat.md`, `skills.md` (combat skills), `equipment-and-encumbrance.md` (tags), `app-decisions.md` |
| Spellcasting UI | `magic.md`, `classes-and-leveling.md`, `app-decisions.md` |
| Arts / Effort UI | `magic.md`, `app-decisions.md` |
| Encumbrance / inventory feature | `equipment-and-encumbrance.md`, `combat.md` (armor/shield interaction), `app-decisions.md` |
| HP / Strain / rest mechanics | `conditions-and-status.md`, `magic.md` (rest resets slots/effort), `app-decisions.md` |
| Saves / save targets | `core-resolution.md`, `classes-and-leveling.md` (specialist vs generalist), `app-decisions.md` |
| Skill-check resolver | `core-resolution.md`, `skills.md`, `app-decisions.md` |
| Anything campaign-scoped (membership, invites, NPC visibility, content authoring, sharing) | `multi-tenancy.md`, `app-decisions.md` |

If your task is purely cosmetic (CSS, copy, refactor) you may skip the rules entirely.
